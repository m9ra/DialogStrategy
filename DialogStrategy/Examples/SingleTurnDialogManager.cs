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
    class SingleTurnDialogManager : DialogManagerBase
    {
        private DialogLayer _currentDialog;

        public SingleTurnDialogManager(params GraphLayerBase[] fixedlayers)
            : base(fixedlayers)
        {
        }

        protected override ComposedGraph createGraph(string utterance)
        {
            _currentDialog = new DialogLayer();
            foreach (var word in GetInput(utterance))
            {
                _currentDialog.Inform(word);
            }

            var graph = new ComposedGraph(FixedLayers.Concat(new[] { _currentDialog }).ToArray());
            return graph;
        }

        protected override NodeReference getInputNode(string nodeRepresentation)
        {
            return _currentDialog.GetNode(nodeRepresentation);
        }

        protected Dictionary<NodeReference, KnowledgePath[]> findPaths(ComposedGraph graph, IEnumerable<NodeReference> inputs, NodeReference result)
        {
            //TODO improve - this is just searching connections between inputs and desired result
            var paths = new Dictionary<NodeReference, KnowledgePath[]>();
            var activeNode = getInputNode(Graph.Active);
            foreach (var input in inputs)
            {
                var separateDialog = new DialogLayer();
                separateDialog.Inform(input);

                var separateGraph = new ComposedGraph(FixedLayers.Concat(new[] { separateDialog }).ToArray());
                var inputPaths = separateGraph.GetPaths(activeNode, result, LenLimit, WideLimit).Take(2);
                paths[input] = inputPaths.ToArray();
            }
            return paths;
        }

        protected override NodeReference getGraphNode(string nodeRepresentation)
        {
            return getInputNode(nodeRepresentation);
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

            if (intersectedPaths.Count > 0)
                return new KnowledgePattern(intersectedPaths);
            else
                return new KnowledgePattern(noIntersectionPaths);
        }
    }
}
