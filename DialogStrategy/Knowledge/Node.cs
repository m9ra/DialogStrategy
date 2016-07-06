using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogStrategy.Knowledge
{
    class Node
    {
        /// <summary>
        /// Outgoing edges from current node indexed by relations.
        /// </summary>
        private readonly Dictionary<string, List<Node>> _outEdges = new Dictionary<string, List<Node>>();

        private readonly Dictionary<string, List<Node>> _inEdges = new Dictionary<string, List<Node>>();

        /// <summary>
        /// Name of node.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Confidence of node.
        /// </summary>
        public readonly double Confidence = 1.0;

        public Node(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Add edge to given node
        /// </summary>
        /// <param name="relation">Relation representing edge</param>
        /// <param name="node">Node that is connected by relation</param>
        internal void SetEdge(string relation, Node node)
        {
            if (relation == null)
                throw new ArgumentNullException("relation");

            if (node == null)
                throw new ArgumentNullException("node");

            List<Node> relationNodes;
            if (!_outEdges.TryGetValue(relation, out relationNodes))
                _outEdges[relation] = relationNodes = new List<Node>();

            relationNodes.Add(node);
            node.reverseEdge(relation, this);
        }


        internal IEnumerable<string> GetEdges(Node nodeTo)
        {
            var result = new List<string>();
            foreach (var pair in _outEdges)
            {
                if (pair.Value.Contains(nodeTo))
                    result.Add(pair.Key);
            }

            return result;
        }

        internal IEnumerable<Node> OutNodes(string edge)
        {
            List<Node> nodes;
            if (_outEdges.TryGetValue(edge, out nodes))
                return nodes;

            return new Node[0];
        }

        internal IEnumerable<Node> InNodes(string edge)
        {
            List<Node> nodes;
            if (_inEdges.TryGetValue(edge, out nodes))
                return nodes;

            return new Node[0];
        }

        private void reverseEdge(string relation, Node node)
        {
            List<Node> relationNodes;
            if (!_inEdges.TryGetValue(relation, out relationNodes))
                _inEdges[relation] = relationNodes = new List<Node>();

            relationNodes.Add(node);
        }

        internal IEnumerable<Node> GetInherited()
        {
            var result = new List<Node>();

            result.Add(this);

            foreach (var inherited in InNodes(Graph.IsRelation))
                foreach (var child in inherited.GetInherited())
                    result.Add(child);

            return result;
        }

        internal Node GetParent()
        {
            return OutNodes(Graph.IsRelation).FirstOrDefault();
        }

        public override string ToString()
        {
            return "<" + Name + ">";
        }
    }
}
