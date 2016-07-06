using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DialogStrategy.Knowledge;

namespace DialogStrategy.Computation.Model
{
    class NodeRestriction
    {
        private readonly List<NodeRestriction> _otherRestrictions = new List<NodeRestriction>();

        private readonly List<string> _restrictionEdges = new List<string>();

        private readonly List<bool> _restrictionOutDirection = new List<bool>();

        public readonly NodeReference BaseNode;

        public int RestrictionCount { get{return _restrictionEdges.Count;}}


        public NodeRestriction(NodeReference baseNode)
        {
            BaseNode = baseNode;
        }

        internal void AddEdge(string edge, bool outDirection, NodeRestriction restrictionTarget)
        {
            if (restrictionTarget == null)
                throw new ArgumentNullException("restrictionTarget");

            _restrictionEdges.Add(edge);
            _restrictionOutDirection.Add(outDirection);
            _otherRestrictions.Add(restrictionTarget);
        }

        internal NodeRestriction GetTarget(int i)
        {
            return _otherRestrictions[i];
        }

        internal string GetEdge(int i)
        {
            return _restrictionEdges[i];
        }

        internal bool IsOutDirection(int i)
        {
            return _restrictionOutDirection[i];
        }
    }
}
