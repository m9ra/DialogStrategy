using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DialogStrategy.Knowledge;
using DialogStrategy.Computation;
using DialogStrategy.Computation.Model;
using DialogStrategy.Computation.Actions;
using DialogStrategy.Dialog;

namespace DialogStrategy.Examples
{
    abstract class DialogManagerBase
    {
        private readonly GraphLayerBase[] _fixedlayers;

        protected readonly int WideLimit = 150;

        protected readonly int LenLimit = 7;

        private List<KnowledgePattern> _patterns = new List<KnowledgePattern>();

        private Dictionary<KnowledgePattern, PatternActionBase> _actions = new Dictionary<KnowledgePattern, PatternActionBase>();

        protected abstract ComposedGraph createGraph(string utterance);

        protected abstract NodeReference getInputNode(string nodeRepresentation);

        protected abstract NodeReference getGraphNode(string nodeRepresentation);

        public IEnumerable<KnowledgePattern> Patterns { get { return _patterns; } }

        protected IEnumerable<GraphLayerBase> FixedLayers { get { return _fixedlayers; } }

        public DialogManagerBase(params GraphLayerBase[] fixedlayers)
        {
            _fixedlayers = fixedlayers.ToArray();
        }

        public ActionContext Ask(string utterance)
        {
            var graph = createGraph(utterance);

            var context = new ActionContext();
            foreach (var pattern in _patterns)
            {
                evaluate(context, pattern, graph);
            }

            return context;
        }

        public void Negate(string utterance)
        {
            var result = Ask(utterance).GetTopNode();
            if (result == null)
                return;

            var graph = createGraph(utterance);
            var pattern = createPattern(graph, GetInput(utterance), result);

            _patterns.Add(pattern);
            _actions[pattern] = new InformAction(result, -1);
        }

        public void Advise(string utterance, string advice)
        {
            var graph = createGraph(utterance);
            var result = getGraphNode(advice);
            var pattern = createPattern(graph, GetInput(utterance), result);

            _patterns.Add(pattern);
            _actions[pattern] = new InformAction(result, 1);
        }

        protected IEnumerable<NodeReference> GetInput(string utterance)
        {
            var words = utterance.Split(' ');
            foreach (var word in words)
            {
                yield return getInputNode(word);
            }
        }

        private void evaluate(ActionContext context, KnowledgePattern pattern, ComposedGraph graph)
        {
            var patternContext = new PatternContext(pattern, graph);
            if (patternContext.IsMatch)
            {
                var discount = getDiscount(patternContext.Substitution, graph);
                _actions[pattern].Apply(context, patternContext.Substitution, discount);
            }
        }

        private double getDiscount(NodesSubstitution substitution, ComposedGraph context)
        {
            var discount = 0.0;
            foreach (var pair in substitution.Pairs)
            {
                var path = context.GetPaths(pair.Key, pair.Value, LenLimit, WideLimit).FirstOrDefault();
                var knowledgeDiscount = path == null ? double.PositiveInfinity : path.Length;
                discount -= knowledgeDiscount;
            }

            return discount / substitution.Count;
        }

        protected abstract KnowledgePattern createPattern(ComposedGraph graph, IEnumerable<NodeReference> inputs, NodeReference result);

        protected void fillIntersection(HashSet<KnowledgePath> workSet, KnowledgePath[] paths1, KnowledgePath[] paths2, IEnumerable<NodeReference> exceptNodes)
        {
            for (var i = 0; i < paths1.Length; ++i)
            {
                var pathI = paths1[i];
                for (var j = 0; j < paths2.Length; ++j)
                {
                    var pathJ = paths2[j];

                    if (hasIntersection(pathI, pathJ, exceptNodes))
                    {
                        workSet.Add(pathI);
                        workSet.Add(pathJ);
                    }
                }
            }
        }

        private bool hasIntersection(KnowledgePath pathI, KnowledgePath pathJ, IEnumerable<NodeReference> exceptNodes)
        {
            var intersection = pathI.Nodes.Intersect(pathJ.Nodes).Except(exceptNodes).ToArray();
            return intersection.Length > 3;
        }

    }
}
