using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DialogStrategy.Knowledge;

namespace DialogStrategy.Computation.Condition
{
    class PatternCondition : ConditionBase
    {
        private readonly PatternNode[] _patternNodes;

        private Dictionary<PatternNode, Node> _evaluations;

        public IEnumerable<PatternNode> PatternNodes { get { return _patternNodes; } }

        internal PatternCondition(IEnumerable<PatternNode> pattern)
        {
            _patternNodes = pattern.ToArray();
        }

        public override double Check(Graph input)
        {
            _evaluations = new Dictionary<PatternNode, Node>();

            var result = 1.0;
            for (var i = 0; i < _patternNodes.Length; ++i)
            {
                var patternNode = _patternNodes[i];

                var value = evaluate(patternNode, input);
                result *= value;

                if (result <= 0)
                    //patternNode cannot be satisfied
                    break;
            }

            //condition is not feasible
            return result;
        }

        public Dictionary<PatternNode, Node> GetLastEvaluation()
        {
            return _evaluations;
        }

        internal PatternCondition SetEdge(PatternNode nodeFrom, string edge, PatternNode nodeTo)
        {
            var clone = Clone();

            var fromIndex = Array.IndexOf(_patternNodes, nodeFrom);
            var toIndex = Array.IndexOf(_patternNodes, nodeTo);

            clone._patternNodes[fromIndex].AddEdge(edge, clone._patternNodes[toIndex]);

            return clone;
        }


        internal PatternCondition SetNode(PatternNode oldNode, string newName)
        {
            var translations = new Dictionary<PatternNode, PatternNode>();
            var ordering = new List<PatternNode>();

            foreach (var node in _patternNodes)
            {
                var name = node == oldNode ? newName : node.Name;
                var copy = new PatternNode(name);
                translations[node] = copy;
                ordering.Add(copy);
            }

            foreach (var node in _patternNodes)
            {
                var copiedNode = translations[node];
                for (var i = 0; i < node.EdgePatterns.Count; ++i)
                {
                    var edge = node.EdgePatterns[i];
                    var target = translations[node.TargetNodes[i]];

                    copiedNode.AddEdge(edge, target);
                }
            }

            var clone = new PatternCondition(ordering);
            return clone;
        }

        private double evaluate(PatternNode pattern, Graph input, IEnumerable<Node> contextNodes = null)
        {
            Node node;
            if (_evaluations.TryGetValue(pattern, out node))
            {
                if (contextNodes == null || contextNodes.Contains(node))
                    return node.Confidence;
                else
                    //node doesn't match context node - it cannot be used
                    return 0;
            }

            if (contextNodes == null)
                contextNodes = input.GetInherited(pattern.Name);

            //we are optimistic - hope that pattern will be satisfied
            //otherwise we will remove the association

            var result = 0.0;
            foreach (var contextNode in contextNodes)
            {
                result = tryEvaluateNode(pattern, input, contextNode);
                if (result > 0)
                    break;
            }

            return result;
        }

        private double tryEvaluateNode(PatternNode pattern, Graph input, Node contextNode)
        {
            if (_evaluations.ContainsValue(contextNode))
                //same value for different pattern nodes is not allowed
                return 0;

            _evaluations[pattern] = contextNode;

            var result = contextNode.Confidence;
            for (var i = 0; i < pattern.EdgePatterns.Count; ++i)
            {
                var edge = pattern.EdgePatterns[i];
                var target = pattern.TargetNodes[i];

                var currentNode = contextNode;
                var edgeEvaluation = 0.0;
                while (currentNode != null)
                {
                    var candidates = currentNode.OutNodes(edge);
                    edgeEvaluation = evaluate(target, input, candidates);
                    if (edgeEvaluation > 0)
                        //we have found evaluation
                        break;

                    currentNode = currentNode.GetParent();
                }
                result *= edgeEvaluation;

                if (result <= 0)
                    //condition hasn't been satisfied
                    break;
            }

            if (result <= 0)
                //evaluation cannot be satisfied
                _evaluations.Remove(pattern);

            return result;
        }

        private PatternCondition Clone()
        {
            var translations = new Dictionary<PatternNode, PatternNode>();
            var ordering = new List<PatternNode>();

            foreach (var node in _patternNodes)
            {
                var copy = new PatternNode(node.Name);
                translations[node] = copy;
                ordering.Add(copy);
            }

            foreach (var node in _patternNodes)
            {
                var copiedNode = translations[node];
                for (var i = 0; i < node.EdgePatterns.Count; ++i)
                {
                    var edge = node.EdgePatterns[i];
                    var target = translations[node.TargetNodes[i]];

                    copiedNode.AddEdge(edge, target);
                }
            }

            return new PatternCondition(ordering);
        }

        public override string ToString()
        {
            return string.Join(" & ", (object[])_patternNodes);
        }
    }
}
