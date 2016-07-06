using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DialogStrategy.Examples;

using DialogStrategy.Knowledge;

namespace DialogStrategy.Dialog
{
    abstract class UtteranceBase
    {
        /// <summary>
        /// Provider that is used for input/output handling
        /// </summary>
        private ConsoleDialogProvider _provider;

        protected abstract void handleManager(DialogManagerBase manager);

        protected AskUtterance LastAskUtterance { get { return _provider.LastAskUtterance; } }

        internal void HandleManager(ConsoleDialogProvider provider)
        {
            _provider = provider;
            try
            {
                handleManager(provider.Manager);
            }
            finally
            {
                _provider = null;
            }
        }

        protected void Print(KnowledgePath path)
        {
            _provider.Print(path);
        }

        protected void Print(KeyValuePair<NodeReference, double> hypothesis)
        {
            _provider.Print(hypothesis);
        }

        protected void Output(string output)
        {
            _provider.Output(output);
        }

        protected void Output(NodeReference node)
        {
            if (node == null)
                _provider.Output("");
            else
                _provider.Output(node.Data.ToString());
        }

        protected void PrintSection(string sectionName)
        {
            _provider.Print(sectionName);
        }

        protected void EndSection()
        {
            _provider.EndSection();
        }
    }

    class AskUtterance : UtteranceBase
    {
        internal readonly string Question;

        private AskUtterance(string question)
        {
            Question = question;
        }

        internal static AskUtterance TryParse(string utterance)
        {
            return new AskUtterance(utterance.TrimEnd('?'));
        }

        protected override void handleManager(DialogManagerBase manager)
        {
            var result = manager.Ask(Question);
            Output(result.GetTopNode());

            var hypotheses = result.hypotheses().OrderByDescending((a) => a.Value);
            PrintSection("Hypotheses");
            foreach (var hypothesis in hypotheses)
            {
                Print(hypothesis);
            }
            EndSection();
        }
    }

    class NoUtterance : UtteranceBase
    {
        internal static NoUtterance TryParse(string utterance)
        {
            if (!utterance.StartsWith("no"))
                return null;

            return new NoUtterance();
        }
        protected override void handleManager(DialogManagerBase manager)
        {
            manager.Negate(LastAskUtterance.Question);

            var pattern = manager.Patterns.Last();

            PrintSection("Detected pattern");
            foreach (var path in pattern.Paths)
            {
                Print(path);
            }
            EndSection();
        }
    }

    class AdviceUtterance : UtteranceBase
    {
        internal readonly string Question;

        internal readonly string Advice;

        private AdviceUtterance(string question, string answer)
        {
            Question = question.Trim();
            Advice = answer.Trim();
        }

        internal static AdviceUtterance TryParse(string utterance)
        {
            if (!utterance.Contains(" is "))
                return null;

            var parts = utterance.Split(new[] { "is" }, 2, StringSplitOptions.RemoveEmptyEntries);

            return new AdviceUtterance(parts[0], parts[1]);
        }

        protected override void handleManager(DialogManagerBase manager)
        {
            manager.Advise(Question, Advice);
            var pattern = manager.Patterns.Last();

            PrintSection("Detected pattern");
            foreach (var path in pattern.Paths)
            {
                Print(path);
            }
            EndSection();
        }
    }
}
