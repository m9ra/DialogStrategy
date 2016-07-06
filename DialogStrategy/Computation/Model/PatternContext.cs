using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DialogStrategy.Knowledge;

namespace DialogStrategy.Computation.Model
{
    class PatternContext
    {
        private readonly KnowledgePattern _pattern;

        private readonly ComposedGraph _contextGraph;

        private readonly Dictionary<NodeReference, NodeRestriction> _currentRestrictions = new Dictionary<NodeReference, NodeRestriction>();

        private readonly Dictionary<NodeReference, NodeAlternatives> _alternativesIndex = new Dictionary<NodeReference, NodeAlternatives>();

        private readonly Dictionary<NodeReference, NodeReference> _substitutions = new Dictionary<NodeReference, NodeReference>();

        private readonly Stack<NodeReference> _alternativesEvaluationStack = new Stack<NodeReference>();

        private readonly NodeReference[] _candidateOrdering;

        public readonly NodesSubstitution Substitution;

        public bool IsMatch { get { return _substitutions.Count > 0; } }

        public PatternContext(KnowledgePattern pattern, ComposedGraph contextGraph)
        {
            this._pattern = pattern;
            this._contextGraph = contextGraph;

            var candidates = new List<NodeReference>();
            foreach (var path in pattern.Paths)
            {
                var previousNode = path.Node(0);
                candidates.Add(previousNode);
                for (var i = 0; i < path.Length; ++i)
                {
                    var currentNode = path.Node(i + 1);
                    candidates.Add(currentNode);

                    reportRestriction(previousNode, path.Edge(i), path.OutDirection(i), currentNode);

                    previousNode = currentNode;
                }
            }

            _candidateOrdering = candidates.ToArray();
            evaluate();

            Substitution = new NodesSubstitution(_substitutions);
        }

        private void reportRestriction(NodeReference node1, string edge, bool outDirection, NodeReference node2)
        {
            var restriction1 = getRestriction(node1);
            var restriction2 = getRestriction(node2);

            restriction1.AddEdge(edge, outDirection, restriction2);
            restriction2.AddEdge(edge, !outDirection, restriction1);
        }

        private NodeRestriction getRestriction(NodeReference node)
        {
            NodeRestriction result;
            if (!_currentRestrictions.TryGetValue(node, out result))
                _currentRestrictions[node] = result = new NodeRestriction(node);

            return result;
        }

        private void evaluate()
        {
            var candidate = getCurrentCandidate();
            push(candidate);

            while (_alternativesEvaluationStack.Count > 0)
            {
                var currentCandidate = _alternativesEvaluationStack.Peek();
                var currentAlternative = getCurrentAlternative(currentCandidate);

                if (currentAlternative == null)
                {
                    //there is no alternative
                    pop();
                }
                else
                {
                    substitute(currentCandidate, currentAlternative);
                    var nextCandidate = getCurrentCandidate();

                    if (nextCandidate == null)
                        //evaluation has been found
                        break;

                    push(nextCandidate);
                }
            }
        }

        private void substitute(NodeReference candidate, NodeReference alternative)
        {
            _substitutions[candidate] = alternative;
        }

        private void push(NodeReference nextCandidate)
        {
            if (nextCandidate == null)
            {
                if (_alternativesEvaluationStack.Count > 0)
                    throw new InvalidOperationException("Cannot push null in current state");

                return;
            }
            _alternativesEvaluationStack.Push(nextCandidate);
        }

        private void pop()
        {
            var popped = _alternativesEvaluationStack.Pop();
            _substitutions.Remove(popped);

            //reset alternative index for popped node
            _alternativesIndex.Remove(popped);
        }

        private NodeReference getCurrentAlternative(NodeReference node)
        {
            NodeAlternatives alternatives;
            if (!_alternativesIndex.TryGetValue(node, out alternatives))
                _alternativesIndex[node] = alternatives = new NodeAlternatives(node, findAlternatives(node));

            NodeReference alternative;
            do
            {
                alternative = alternatives.Next();
            } while (alternative != null && _substitutions.Values.Contains(alternative));

            return alternative;
        }

        private IEnumerable<NodeReference> findAlternatives(NodeReference node)
        {
            var alternatives = new List<Tuple<NodeReference, int>>();
            var consistentAlternatives = getConsistentAlternatives(node);

            foreach (var alternative in consistentAlternatives)
            {
                var weakConditionScore = getWeakConditionScore(node, alternative);
                alternatives.Add(Tuple.Create(alternative, weakConditionScore));
            }

            alternatives.Sort((a, b) => b.Item2.CompareTo(a.Item2));

            //sorted by count of weak conditions that are satisfied
            return from alternativeItem in alternatives select alternativeItem.Item1;
        }

        private IEnumerable<NodeReference> getConsistentAlternatives(NodeReference node)
        {
            var rawAlternatives = rawOrderedAlternatives(node);
            foreach (var rawAlternative in rawAlternatives)
            {
                if (isContextConsistent(node, rawAlternative))
                    yield return rawAlternative;
            }
        }

        /// <summary>
        /// Get alternatives ordered by alternative distance
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IEnumerable<NodeReference> rawOrderedAlternatives(NodeReference node)
        {
            //node itself can be an alternative
            yield return node;

            var alternativesLimit = 20;
            var edgeIterators = getStrongEdgeIterators(node);
            while (edgeIterators.Count > 0)
            {
                for (var i = 0; i < edgeIterators.Count; ++i)
                {
                    var iterator = edgeIterators[i];
                    if (!iterator.MoveNext())
                    {
                        //there are no next edges
                        edgeIterators.RemoveAt(i);
                        --i;
                        continue;
                    }

                    yield return iterator.Current;

                    //limit count of alternatives
                    --alternativesLimit;
                    if (alternativesLimit < 0)
                        yield break;
                }
            }
        }

        private List<IEnumerator<NodeReference>> getStrongEdgeIterators(NodeReference node)
        {
            var result = new List<IEnumerator<NodeReference>>();
            var nodeRestriction = getRestriction(node);

            //iterate over strong edges
            for (var i = 0; i < nodeRestriction.RestrictionCount; ++i)
            {
                var targetRestriction = nodeRestriction.GetTarget(i);
                var targetSubstitution = getSubstitution(targetRestriction.BaseNode);
                if (targetSubstitution == null)
                    //there is no substitution yet - it is weak neighbour
                    continue;

                var edge = nodeRestriction.GetEdge(i);
                var isOutDirection = nodeRestriction.IsOutDirection(i);


                result.Add(getEdgeIterator(targetSubstitution, edge, !isOutDirection).GetEnumerator());
            }

            return result;
        }

        /// <summary>
        /// Iterates over alternatives to fromNodes according to edge between given nodes.
        /// </summary>
        /// <param name="fromNode"></param>
        /// <param name="toNode"></param>
        /// <returns></returns>
        private IEnumerable<NodeReference> getEdgeIterator(NodeReference targetSubstitution, string edge, bool isOutDirection)
        {
            if (isOutDirection)
                return _contextGraph.OutcommingTargets(targetSubstitution, edge);
            else
                return _contextGraph.IncommingTargets(targetSubstitution, edge);
        }

        private IEnumerable<NodeReference> getStrongNeighbours(NodeReference node)
        {
            var result = new List<NodeReference>();
            var nodeRestriction = getRestriction(node);
            for (var i = 0; i < nodeRestriction.RestrictionCount; ++i)
            {
                var targetRestriction = nodeRestriction.GetTarget(i);
                var targetSubstitution = getSubstitution(targetRestriction.BaseNode);
                if (targetSubstitution == null)
                    //there is no substitution yet - it is weak neighbour
                    continue;

                result.Add(targetSubstitution);
            }

            return result;
        }

        private NodeReference getCurrentCandidate()
        {
            for (var i = 0; i < _candidateOrdering.Length; ++i)
            {
                var candidate = _candidateOrdering[i];
                if (!_substitutions.ContainsKey(candidate))
                    return candidate;
            }

            return null;
        }

        private bool isContextConsistent(NodeReference node, NodeReference alternative)
        {
            var nodeRestriction = getRestriction(node);
            for (var i = 0; i < nodeRestriction.RestrictionCount; ++i)
            {
                var targetRestriction = nodeRestriction.GetTarget(i);
                var targetSubstitution = getSubstitution(targetRestriction.BaseNode);
                if (targetSubstitution == null)
                    //there is no substitution yet - weak condition cannot violate consistency
                    continue;

                var edge = nodeRestriction.GetEdge(i);
                var isOutDirection = nodeRestriction.IsOutDirection(i);

                if (!_contextGraph.HasEdge(alternative, edge, isOutDirection, targetSubstitution))
                    //inconsitency has been found
                    return false;
            }

            return true;
        }

        private NodeReference getSubstitution(NodeReference node)
        {
            NodeReference result;
            _substitutions.TryGetValue(node, out result);
            return result;
        }

        private int getWeakConditionScore(NodeReference node, NodeReference alternative)
        {
            var score = 0;
            var nodeRestriction = getRestriction(node);
            for (var i = 0; i < nodeRestriction.RestrictionCount; ++i)
            {
                var targetRestriction = nodeRestriction.GetTarget(i);
                var targetSubstitution = getSubstitution(targetRestriction.BaseNode);
                if (targetSubstitution != null)
                    //strong condition is not relevant in view of weak score
                    continue;

                var edge = nodeRestriction.GetEdge(i);
                var isOutDirection = nodeRestriction.IsOutDirection(i);

                if (_contextGraph.HasEdge(alternative, edge, isOutDirection, node))
                    //inconsitency has been found
                    ++score;
            }

            return score;
        }
    }
}
