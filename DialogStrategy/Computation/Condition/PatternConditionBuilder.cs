using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DialogStrategy.Knowledge;

namespace DialogStrategy.Computation.Condition
{
    class PatternConditionBuilder
    {
        /// <summary>
        /// Node pattern conditions.
        /// </summary>
        private readonly Dictionary<string, PatternNode> _nodes = new Dictionary<string, PatternNode>();

        private readonly List<PatternNode> _createdNodes = new List<PatternNode>();

        private PatternNode _contextNode;

        private string _contextEdge;

        #region Essential build methods

        internal PatternConditionBuilder SetContextNode(string nodeName)
        {
            if (_contextEdge != null)
                throw new NotSupportedException("Cannot change context node with pending edge request");

            _contextNode = getOrCreate(nodeName);
            return this;
        }

        internal PatternConditionBuilder Node(string nodeName, bool createAnother = false)
        {
            var describedNode = getOrCreate(nodeName, createAnother);

            if (_contextEdge != null)
            {
                //add edge to node from context
                _contextNode.AddEdge(_contextEdge, describedNode);
                _contextEdge = null;
            }

            return SetContextNode(nodeName);
        }

        internal PatternConditionBuilder Edge(string edgeName)
        {
            if (edgeName == null)
                throw new ArgumentNullException("edgeName");

            if (_contextEdge != null)
                throw new NotSupportedException("Cannot build two edges at once");

            _contextEdge = edgeName;

            return this;
        }

        internal PatternCondition Build()
        {
            if (_contextEdge != null)
                throw new NotSupportedException("Cannot build condition when edge request is pending");

            return new PatternCondition(_createdNodes);
        }

        #endregion

        #region Build helpers

        internal PatternConditionBuilder ActiveChild(string nodeName, bool createAnother = false)
        {
            Node(nodeName, createAnother).HasFlag(Graph.Active);
            return SetContextNode(nodeName);
        }

        internal PatternConditionBuilder HasFlag(string flagName)
        {
            if (_contextNode == null)
                throw new NotSupportedException("Node has to be selected before setting its flag");

            var flaggedNodeName = _contextNode.Name;

            Edge(Graph.HasFlag).Node(flagName);

            return SetContextNode(flaggedNodeName);
        }

        #endregion

        #region Private utilities

        private PatternNode getOrCreate(string nodeName, bool createAnother = false)
        {
            PatternNode node;
            if (!_nodes.TryGetValue(nodeName, out node) || createAnother)
            {
                _nodes[nodeName] = node = new PatternNode(nodeName);
                _createdNodes.Add(node);
            }

            return node;
        }

        #endregion

    }
}
