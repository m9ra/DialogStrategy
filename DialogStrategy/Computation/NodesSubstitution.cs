using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DialogStrategy.Knowledge;

namespace DialogStrategy.Computation
{
    class NodesSubstitution
    {
        private readonly Dictionary<NodeReference, NodeReference> _substitutions;

        public IEnumerable<KeyValuePair<NodeReference, NodeReference>> Pairs { get { return _substitutions; } }

        public int Count { get { return _substitutions.Count; } }

        public NodesSubstitution(Dictionary<NodeReference, NodeReference> substitutions)
        {
            _substitutions = new Dictionary<NodeReference, NodeReference>(substitutions);
        }

        internal NodeReference Get(NodeReference node)
        {
            NodeReference substitution;
            if(!_substitutions.TryGetValue(node,out substitution))
                return node;

            return substitution;
        }
    }
}
