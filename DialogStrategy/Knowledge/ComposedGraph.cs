using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogStrategy.Knowledge
{
    class ComposedGraph
    {
        private readonly GraphLayerBase[] _layers;

        public ComposedGraph(params GraphLayerBase[] layers)
        {
            _layers = layers.ToArray();
        }


        public NodeReference GetNode(object data)
        {
            return new NodeReference(data);
        }

        public bool HasEdge(NodeReference fromNode, string edge, bool isOutDirection, NodeReference toNode)
        {
            if (isOutDirection)
                return HasEdge(fromNode, edge, toNode);
            else
                return HasEdge(toNode, edge, fromNode);
        }

        public bool HasEdge(NodeReference fromNode, string edge, NodeReference toNode)
        {
            foreach (var layer in _layers)
            {
                var edges = layer.Edges(fromNode, toNode);
                if (edges.Contains(edge))
                    return true;
            }

            return false;
        }


        internal IEnumerable<NodeReference> OutcommingTargets(NodeReference fromNode, string edge)
        {
            foreach (var layer in _layers)
            {
                foreach (var node in layer.Outcoming(fromNode, edge))
                {
                    yield return node;
                }
            }
        }

        internal IEnumerable<NodeReference> IncommingTargets(NodeReference toNode, string edge)
        {
            foreach (var layer in _layers)
            {
                foreach (var node in layer.Incoming(toNode, edge))
                {
                    yield return node;
                }
            }
        }

        public IEnumerable<KnowledgePath> GetPaths(NodeReference from, NodeReference to, int maxLength, int maxWidth)
        {
            var currentQueue = new Queue<PathSegment>();
            var visitedNodes = new HashSet<NodeReference>();

            var startSegment = new PathSegment(null, null, from);
            if (from.Equals(to))
            {
                yield return new KnowledgePath(this, startSegment);
                yield break;
            }


            //starting node
            currentQueue.Enqueue(startSegment);
            //delimiter - for counting path length
            currentQueue.Enqueue(null);

            visitedNodes.Add(from);
            visitedNodes.Add(to);

            var currentPathLength = 0;
            while (currentQueue.Count > 0 && currentPathLength < maxLength)
            {
                var currentSegment = currentQueue.Dequeue();
                if (currentSegment == null)
                {
                    ++currentPathLength;
                    //add next delimiter
                    currentQueue.Enqueue(null);
                    continue;
                }

                //test if we can get into end node
                foreach (var edge in BetweenEdges(currentSegment.Node, to))
                {
                    var segment = new PathSegment(currentSegment, edge, to);
                    yield return new KnowledgePath(this, segment);
                }

                //explore next children
                foreach (var childPair in getChildren(currentSegment.Node, maxWidth))
                {
                    var edge = childPair.Key;
                    var child = childPair.Value;
                    if (!visitedNodes.Add(child))
                        //this node has already been visited
                        continue;

                    var childSegment = new PathSegment(currentSegment, edge, child);
                    currentQueue.Enqueue(childSegment);
                }
            }
        }

        /// <summary>
        /// Edges between given nodes (both side edges).
        /// </summary>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        /// <returns></returns>
        public IEnumerable<string> BetweenEdges(NodeReference node1, NodeReference node2)
        {
            foreach (var layer in _layers)
            {
                foreach (var edge in layer.Edges(node1, node2))
                    yield return edge;

                foreach (var edge in layer.Edges(node2, node1))
                    yield return edge;
            }
        }

        private IEnumerable<KeyValuePair<string, NodeReference>> getChildren(NodeReference node, int maxWidth)
        {
            foreach (var layer in _layers)
            {
                foreach (var pair in layer.Incoming(node).Take(maxWidth))
                    yield return pair;

                foreach (var pair in layer.Outcoming(node).Take(maxWidth))
                    yield return pair;
            }
        }
    }
}
