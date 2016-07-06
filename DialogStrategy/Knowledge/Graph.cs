using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogStrategy.Knowledge
{
    class Graph
    {
        #region Standard relations

        public static readonly string IsRelation = "is";

        public static readonly string HasFlag = "has flag";

        #endregion

        #region Standard nodes

        public static readonly string Flag = "flag";

        public static readonly string Active = "active";

        #endregion

        private readonly Random rnd = new Random(1);

        /// <summary>
        /// Index of currently available nodes.
        /// </summary>
        private readonly Dictionary<string, Node> _index = new Dictionary<string, Node>();

        /// <summary>
        /// Add described knowledge.
        /// </summary>
        /// <param name="node1Name">Node which relation will be set.</param>
        /// <param name="relation">Relation that will be set.</param>
        /// <param name="node2Name">Node that is connected by the relation.</param>
        public Graph Add(string node1Name, string relation, string node2Name)
        {
            var node2 = GetOrCreateNode(node2Name);
            GetOrCreateNode(node1Name).SetEdge(relation, node2);
            return this;
        }

        /// <summary>
        /// Get or create node with given name.
        /// </summary>
        /// <param name="name">Name of required node.</param>
        /// <returns>Node with required name.s</returns>
        public Node GetOrCreateNode(string name)
        {
            Node node;
            if (_index.TryGetValue(name, out node))
                return node;

            node = new Node(name);
            _index[name] = node;
            return node;
        }

        /// <summary>
        /// Get children (and node itself) available according to Is edge
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal IEnumerable<Node> GetInherited(string name)
        {
            Node currentNode;
            _index.TryGetValue(name, out currentNode);

            if (currentNode == null)
                return new Node[0];

            return currentNode.GetInherited();
        }

        internal IEnumerable<Node> GetParents(string name)
        {
            var currentNode = GetOrCreateNode(name);
            var result = new List<Node>();

            while (currentNode != null)
            {
                result.Add(currentNode);

                currentNode = currentNode.GetParent();
            }

            return result;
        }

        internal string GetRandomNode()
        {
            var keys = _index.Keys.ToArray();
            return keys[rnd.Next(keys.Length)];
        }
    }
}
