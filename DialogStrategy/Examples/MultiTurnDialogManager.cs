using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DialogStrategy.Learning;
using DialogStrategy.Knowledge;
using DialogStrategy.Computation;
using DialogStrategy.Computation.Model;
using DialogStrategy.Computation.Actions;

namespace DialogStrategy.Examples
{
    class MultiTurnDialogManager : DialogManagerBase
    {
        private MultiDialogLayer _currentDialog;

        public MultiTurnDialogManager(params GraphLayerBase[] fixedlayers)
            : base(fixedlayers)
        {
            _currentDialog = new MultiDialogLayer();
        }

        protected override ComposedGraph createGraph(string utterance)
        {
            _currentDialog.InformSentence(utterance.Split(' '));

            var graph = new ComposedGraph(FixedLayers.Concat(new[] { _currentDialog }).ToArray());
            return graph;
        }

        protected override NodeReference getInputNode(string nodeRepresentation)
        {
            return _currentDialog.GetInputNode(nodeRepresentation);
        }

        protected Dictionary<NodeReference, KnowledgePath[]> findPaths(ComposedGraph graph, IEnumerable<NodeReference> inputs, NodeReference result)
        {
            //TODO improve - this is just searching connections between inputs and desired result
            var paths = new Dictionary<NodeReference, KnowledgePath[]>();
            var activeNode = getGraphNode(Graph.Active);
            foreach (var input in inputs)
            {
                var inputPaths = graph.GetPaths(input, result, 10, WideLimit).Take(2);
                var pathResult = inputPaths.ToArray();

                for (var i = 0; i < pathResult.Length; ++i)
                {
                    var path = pathResult[i];
                    var activePath = _currentDialog.Activate(path);
                    pathResult[i] = activePath;
                }

                paths[input] = pathResult;
            }
            return paths;
        }

        protected override NodeReference getGraphNode(string nodeRepresentation)
        {
            return _currentDialog.GetResultNode(nodeRepresentation);
        }

        protected override KnowledgePattern createPattern(ComposedGraph graph, IEnumerable<NodeReference> inputs, NodeReference result)
        {
            var paths = findPaths(graph, inputs, result);

            var noIntersectionPaths = new List<KnowledgePath>();
            foreach (var path in paths.Values)
                noIntersectionPaths.AddRange(path);

            var intersectedPaths = new HashSet<KnowledgePath>();
            var inputsArray = inputs.ToArray();
            var exceptNodes = new[] { result }.Concat(inputs);
            for (var i = 0; i < inputsArray.Length; ++i)
            {
                for (var j = i + 1; j < inputsArray.Length; ++j)
                {
                    fillIntersection(intersectedPaths, paths[inputsArray[i]], paths[inputsArray[j]], exceptNodes);
                }
            }

            var resultPaths = intersectedPaths.Count > 0 ? intersectedPaths.ToArray() : noIntersectionPaths.ToArray();
            resultPaths = improvePaths(resultPaths, graph);

            return new KnowledgePattern(resultPaths);
        }

        private KnowledgePath[] improvePaths(KnowledgePath[] resultPaths, ComposedGraph graph)
        {
            //throw new NotImplementedException();
            return resultPaths;
        }
    }
}
