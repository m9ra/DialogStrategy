using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DialogStrategy.Knowledge;

namespace DialogStrategy.Examples
{
    class MultiDialogLayer : GeneratedLayer
    {
        public readonly static string SentenceParent = "sentence";

        public readonly static string HasWordRelation = "has word";

        public readonly static string NextRelation = "next";

        private Dictionary<string, string[]> _sentences = new Dictionary<string, string[]>();

        public void InformSentence(string[] words)
        {
            var newSentence = "sentence" + _sentences.Count;
            _sentences.Add(newSentence, words.ToArray());

            SetEdge(newSentence, Graph.IsRelation, SentenceParent);

            string previousWordInstance = null;
            foreach (var word in words)
            {
                var wordInstance = getWordInstance(newSentence, word);
                var wordNode = getKnowledgeNode(word);


                SetEdge(newSentence, HasWordRelation, wordInstance);
                if (previousWordInstance != null)
                    SetEdge(previousWordInstance, NextRelation, wordInstance);

                SetEdge(wordInstance, Graph.IsRelation, wordNode.Data);
                previousWordInstance = wordInstance;
            }

            if (_sentences.Count > 1)
            {
                var previousActive = "sentence" + (_sentences.Count - 2);
                RemoveFromEdge(previousActive,Graph.HasFlag);
            }

            SetEdge(newSentence, Graph.HasFlag, Graph.Active);
        }

        internal NodeReference GetInputNode(string wordRepresentation)
        {
            if (wordRepresentation != Graph.Active)
            {
                var currentSentence = "sentence" + (_sentences.Count - 1);
                wordRepresentation = getWordInstance(currentSentence, wordRepresentation);
            }
            return CreateReference(wordRepresentation);
        }

        private NodeReference getKnowledgeNode(string representation)
        {
            int value;
            if (int.TryParse(representation, out value))
                return CreateReference(value);

            return CreateReference(representation);
        }

        private string getWordInstance(string sentence, string word)
        {
            return sentence + "." + word;
        }

        internal NodeReference GetResultNode(string nodeRepresentation)
        {
            int value;
            if (int.TryParse(nodeRepresentation, out value))
                return CreateReference(value);

            return CreateReference(nodeRepresentation);
        }

        internal KnowledgePath Activate(KnowledgePath path)
        {
            var currentSentence = "sentence" + (_sentences.Count - 1);

            var activePath = path
                        .PrependBy(CreateReference(currentSentence), HasWordRelation, true)
                        .PrependBy(CreateReference(Graph.Active), Graph.HasFlag, false);

            return activePath;
        }
    }
}
