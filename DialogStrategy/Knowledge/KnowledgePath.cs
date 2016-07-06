using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogStrategy.Knowledge
{
    class KnowledgePath
    {
        private readonly PathSegment _lastSegment;

        private readonly List<NodeReference> _nodes = new List<NodeReference>();

        private readonly List<string> _edges = new List<string>();

        /// <summary>
        /// Determine whether edge is in direction (true) or reversed (false)
        /// </summary>
        private readonly List<bool> _edgeDirection = new List<bool>();

        public readonly int Length;

        public IEnumerable<NodeReference> Nodes { get { return _nodes; } }

        public IEnumerable<string> Edges { get { return _edges; } }

        internal KnowledgePath(ComposedGraph graph, PathSegment lastSegment)
        {
            _lastSegment = lastSegment;

            var previousSegment = _lastSegment;
            //add first node that has no outgoing edge
            _nodes.Add(previousSegment.Node);

            var currentSegment = previousSegment.PreviousSegment;
            while (currentSegment != null)
            {
                var previousNode = previousSegment.Node;
                var currentNode = currentSegment.Node;
                var currentEdge = previousSegment.Edge;

                _nodes.Add(currentNode);
                _edges.Add(currentEdge);
                _edgeDirection.Add(!graph.HasEdge(previousNode, currentEdge, currentNode));

                previousSegment = currentSegment;
                currentSegment = currentSegment.PreviousSegment;
            }

            _nodes.Reverse();
            _edges.Reverse();
            _edgeDirection.Reverse();

            Length = _edges.Count;
        }

        private KnowledgePath(IEnumerable<NodeReference> nodes, IEnumerable<string> edges, IEnumerable<bool> edgeDirection)
        {
            _nodes.AddRange(nodes);
            _edges.AddRange(edges);
            _edgeDirection.AddRange(edgeDirection);

            Length = _edges.Count;
        }

        public override string ToString()
        {
            var result = new StringBuilder();

            for (var i = 0; i < _edges.Count; ++i)
            {
                var node = _nodes[i];
                var edge = _edges[i];
                var direction = _edgeDirection[i];

                result.Append(node.Data);

                result.Append(direction ? " --" : " <-");

                result.Append(edge);

                result.Append(direction ? "-> " : "-- ");
            }

            result.Append(_nodes[_nodes.Count - 1].Data);

            return result.ToString();
        }

        internal NodeReference Node(int index)
        {
            return _nodes[index];
        }

        internal bool OutDirection(int index)
        {
            return _edgeDirection[index];
        }

        internal string Edge(int index)
        {
            return _edges[index];
        }

        internal KnowledgePath PrependBy(NodeReference node, string edge, bool isOutcome)
        {
            return new KnowledgePath(
                new[] { node }.Concat(_nodes),
                new[] { edge }.Concat(_edges),
                new[] { isOutcome }.Concat(_edgeDirection)
                );
        }
    }

    class PathSegment
    {
        public readonly PathSegment PreviousSegment;

        public readonly NodeReference Node;

        public readonly string Edge;

        public PathSegment(PathSegment previousSegment, string edge, NodeReference toNode)
        {
            PreviousSegment = previousSegment;
            Edge = edge;
            Node = toNode;
        }
    }
}
