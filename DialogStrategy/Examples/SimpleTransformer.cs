using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DialogStrategy.Learning;
using DialogStrategy.Knowledge;
using DialogStrategy.Computation;
using DialogStrategy.Computation.Model;
using DialogStrategy.Computation.Condition;

namespace DialogStrategy.Examples
{
    class SimpleTransformer : FunctionTransformationBase
    {
        private readonly Random _rnd = new Random(1);

        internal override IEnumerable<ConditionalFunction> GetTransformations(ConditionalFunction function, AnotatedData failingData)
        {
            var result = new List<ConditionalFunction>();

            result.AddRange(addConditionNodes(function, failingData));
            result.AddRange(addConditionEdge(function, failingData));
            result.AddRange(abstractNodes(function, failingData));
            result.AddRange(addActiveFlag(function, failingData));

            var currentTransforms = result.ToArray();
            foreach (var current in currentTransforms)
            {
                result.AddRange(addInstructions(current, failingData));
            }

            result.AddRange(addInstructions(function, failingData));
            return result;
        }

        private IEnumerable<ConditionalFunction> abstractNodes(ConditionalFunction function, AnotatedData data)
        {
            foreach (var patternNode in function.Condition.PatternNodes)
            {
                var parents = data.Graph.GetParents(patternNode.Name);
                foreach (var parent in parents)
                {
                    if (parent.Name == patternNode.Name)
                        continue;

                    var newCondition = changeNode(function.Condition, patternNode, parent);
                    var translatedInstructions = translate(function.Condition, newCondition, function.Instructions);

                    yield return new ConditionalFunction(newCondition, translatedInstructions);
                }
            }
        }

        private IEnumerable<ConditionalFunction> addInstructions(ConditionalFunction function, AnotatedData data)
        {
            foreach (var informedAction in data.GetInformed())
            {
                var parents = data.Graph.GetParents(informedAction.Node.Name).ToArray();
                foreach (var patternNode in function.Condition.PatternNodes)
                {
                    if (!parents.Any((p) => patternNode.Name == p.Name))
                        continue;

                    if (function.Instructions.Where((i) => i is InformInstruction).Select((i) => i as InformInstruction).Any((i) => i.Node == patternNode))
                        continue;

                    yield return new ConditionalFunction(function.Condition, addInform(function.Instructions, patternNode));
                }
            }
        }

        private IEnumerable<ConditionalFunction> addConditionNodes(ConditionalFunction function, AnotatedData data)
        {
            var nodes = new HashSet<Node>(discoverNodes(data));
            foreach (var patternNode in function.Condition.PatternNodes)
            {
                var node = data.Graph.GetOrCreateNode(patternNode.Name);
                nodes.Remove(node);
            }

            foreach (var node in nodes)
            {
                yield return new ConditionalFunction(addNode(function.Condition, node), function.Instructions);
            }
        }

        private IEnumerable<InstructionBase> addInform(IEnumerable<InstructionBase> instructions, PatternNode node)
        {
            return instructions.Concat(new[] { new InformInstruction(node) });
        }

        private PatternCondition addNode(PatternCondition patternCondition, Node node)
        {
            return new PatternCondition(patternCondition.PatternNodes.Union(new[] { new PatternNode(node.Name) }));
        }

        private PatternCondition changeNode(PatternCondition patternCondition, PatternNode patternNode, Node node)
        {
            return patternCondition.SetNode(patternNode, node.Name);
        }

        private IEnumerable<ConditionalFunction> addConditionEdge(ConditionalFunction function, AnotatedData data)
        {
            var nodes = function.Condition.PatternNodes.ToArray();

            foreach (var nodeFrom in nodes)
            {
                if (nodeFrom.EdgePatterns.Count > nodes.Length)
                    //prevent too many edges
                    continue;

                foreach (var nodeTo in nodes)
                {
                    if (nodeFrom == nodeTo)
                        continue;

                    var edges = getEdges(nodeFrom.Name, nodeTo.Name, data);
                    foreach (var edge in edges)
                    {
                        if (edge == Graph.IsRelation)
                            continue;

                        var newCondition = function.Condition.SetEdge(nodeFrom, edge, nodeTo);
                        var translatedInstructions = translate(function.Condition, newCondition, function.Instructions);

                        yield return new ConditionalFunction(newCondition, translatedInstructions);
                    }
                }
            }
        }

        private IEnumerable<ConditionalFunction> addActiveFlag(ConditionalFunction function, AnotatedData data)
        {
            var nodes = function.Condition.PatternNodes;
            var active = nodes.Where((n) => n.Name == Graph.Active).FirstOrDefault();

            var condition = function.Condition;
            if (active == null)
            {
                condition = addNode(condition, data.Graph.GetOrCreateNode(Graph.Active));
                active = condition.PatternNodes.Last();
            }


            foreach (var nodeFrom in nodes)
            {
                if (nodeFrom.TargetNodes.Any((p) => p.Name == Graph.Active))
                    continue;

                var newCondition = condition.SetEdge(nodeFrom, Graph.HasFlag, active);
                var translatedInstructions = translate(condition, newCondition, function.Instructions);

                yield return new ConditionalFunction(newCondition, translatedInstructions);
            }

        }

        private IEnumerable<InstructionBase> translate(PatternCondition oldCondition, PatternCondition newCondition, IEnumerable<InstructionBase> oldInstructions)
        {
            var oldTargets = oldCondition.PatternNodes.ToArray();
            var newTargets = newCondition.PatternNodes.ToArray();

            var translation = new Dictionary<PatternNode, PatternNode>();
            for (var i = 0; i < oldTargets.Length; ++i)
            {
                translation[oldTargets[i]] = newTargets[i];
            }

            var newInstructions = new List<InstructionBase>();
            foreach (var instruction in oldInstructions)
            {
                var inform = instruction as InformInstruction;
                if (inform == null)
                    throw new NotImplementedException();

                newInstructions.Add(new InformInstruction(translation[inform.Node]));
            }

            return newInstructions;
        }

        private IEnumerable<string> getEdges(string nameFrom, string nameTo, AnotatedData data)
        {
            var graph = data.Graph;
            var nodeFrom = graph.GetOrCreateNode(nameFrom);
            var nodeTo = graph.GetOrCreateNode(nameTo);

            return nodeFrom.GetEdges(nodeTo);
        }

        private IEnumerable<Node> discoverNodes(AnotatedData data)
        {
            var startingNodes = new List<string>();
            foreach (var action in data.Anotation)
            {
                startingNodes.Add(action.Node.Name);
            }

            string node = data.Graph.GetRandomNode();
            startingNodes.Add(node);

            return discoverNodes(startingNodes, data.Graph);
        }

        private IEnumerable<Node> discoverNodes(IEnumerable<string> startingNodes, Graph graph)
        {
            var result = new List<Node>();

            foreach (var name in startingNodes)
            {
                result.AddRange(graph.GetParents(name));
            }

            return result;
        }
    }
}
