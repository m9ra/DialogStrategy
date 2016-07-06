using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DialogStrategy.Knowledge;

namespace DialogStrategy.Computation.Actions
{
    class InformAction : PatternActionBase
    {
        private readonly NodeReference _patternNode;

        private readonly double _parameter;

        public InformAction(NodeReference patternNode, double parameter)
        {
            _patternNode = patternNode;
            _parameter = parameter;
        }

        public override void Apply(ActionContext context, NodesSubstitution substitution, double factor)
        {
            var node = substitution.Get(_patternNode);
            var score = 1 / (1 - factor);
            context.Hypothesis(node, _parameter * score);
        }
    }
}
