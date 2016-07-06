using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DialogStrategy.Examples;
using DialogStrategy.Knowledge;

namespace DialogStrategy.Dialog
{
    class ConsoleDialogProvider
    {
        private Queue<string> _utteranceQueue = new Queue<string>();

        internal readonly DialogManagerBase Manager;

        internal AskUtterance LastAskUtterance { get; private set; }

        private static readonly Func<string, UtteranceBase>[] _utteranceParsers = new Func<string, UtteranceBase>[]{
            AdviceUtterance.TryParse,
            NoUtterance.TryParse,
            AskUtterance.TryParse
        };

        public ConsoleDialogProvider(DialogManagerBase manager)
        {
            Manager = manager;
        }

        public void SimulateInput(params string[] utterances)
        {
            foreach(var utterance in utterances){
                _utteranceQueue.Enqueue(utterance);
            }
        }

        public void Run()
        {
            for (; ; )
            {
                var utterance = readUtterance();
                var parsedUtterance = parseUtterance(utterance);
                if (parsedUtterance == null)
                    return;

                parsedUtterance.HandleManager(this);

                if (parsedUtterance is AskUtterance)
                    LastAskUtterance = parsedUtterance as AskUtterance;
            }
        }

        #region Dialog routines

        private string readUtterance()
        {
            printPrompt();

            var lastColor = Console.ForegroundColor;
            Console.ForegroundColor = ActiveColor;

            string utterance;
            if (_utteranceQueue.Count > 0)
            {
                utterance = _utteranceQueue.Dequeue();
                println(utterance, ActiveColor);
            }
            else
            {
                utterance = Console.ReadLine();
            }

            Console.ForegroundColor = lastColor;
            return utterance;
        }

        private UtteranceBase parseUtterance(string utterance)
        {
            //handle console commands
            switch (utterance)
            {
                case "end":
                case "exit":
                case "esc":
                    return null;
            }

            foreach (var parser in _utteranceParsers)
            {
                var result = parser(utterance);
                if (result != null)
                    return result;
            }

            return null;
        }

        #endregion

        #region Printing services

        readonly ConsoleColor SectionBoundaries = ConsoleColor.Cyan;

        readonly ConsoleColor PromptColor = ConsoleColor.Gray;

        readonly ConsoleColor ActiveColor = ConsoleColor.White;

        readonly ConsoleColor NodeColor = ConsoleColor.Yellow;

        readonly ConsoleColor OperatorColor = ConsoleColor.Red;

        readonly ConsoleColor ConfidenceColor = ConsoleColor.DarkCyan;

        private int indentationLevel = 0;

        private bool needIndent = true;

        private readonly Stack<string> _sectionNames = new Stack<string>();

        internal void Print(KeyValuePair<NodeReference, double> hypothesis)
        {
            print(hypothesis.Key.Data, NodeColor);
            print(": ", OperatorColor);
            println(hypothesis.Value, ConfidenceColor);
        }

        internal void EndSection()
        {
            --indentationLevel;
            var borderLength = _sectionNames.Pop().Length + 8;
            println("".PadLeft(borderLength, '='), SectionBoundaries);
        }

        internal void Print(string sectionName)
        {
            println("".PadLeft(4, '=') + sectionName + "".PadLeft(4, '='), SectionBoundaries);
            ++indentationLevel;

            _sectionNames.Push(sectionName);
        }

        internal void Print(KnowledgePath path)
        {
            println(path.ToString(), NodeColor);
        }

        internal void Output(string output)
        {
            print("output< ", PromptColor);
            println(output, ActiveColor);
        }

        private void printPrompt()
        {
            print("\nutterance> ", PromptColor);
        }

        private void println(object data, ConsoleColor color)
        {
            print(data + Environment.NewLine, color);
            needIndent = true;
        }

        private void print(object data, ConsoleColor color)
        {
            var lastColor = Console.ForegroundColor;

            Console.ForegroundColor = color;

            if (needIndent)
            {
                needIndent = false;
                Console.Write("".PadLeft(indentationLevel * 2) + data);
            }
            else
            {
                Console.Write(data);
            }
            Console.ForegroundColor = lastColor;
        }

        #endregion
    }
}
