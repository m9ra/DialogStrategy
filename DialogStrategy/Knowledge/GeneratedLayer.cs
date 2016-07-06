using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogStrategy.Knowledge
{
    class GeneratedLayer : GraphLayerBase
    {
        private readonly Dictionary<object, Dictionary<object, List<string>>> _staticInEdges = new Dictionary<object, Dictionary<object, List<string>>>();

        private readonly Dictionary<object, Dictionary<object, List<string>>> _staticOutEdges = new Dictionary<object, Dictionary<object, List<string>>>();

        private readonly Dictionary<object, Dictionary<NodeGenerator, List<string>>> _staticInGeneratorEdges = new Dictionary<object, Dictionary<NodeGenerator, List<string>>>();

        private readonly Dictionary<NodeGenerator, Dictionary<object, List<string>>> _staticOutGeneratorEdges = new Dictionary<NodeGenerator, Dictionary<object, List<string>>>();

        private readonly List<NodeGenerator> _generators = new List<NodeGenerator>();

        protected void AddNode(object nodeData)
        {
            //there is nothing to do now (we dont need to store static nodes until they dont have edges)
        }

        protected void SetEdge(object node1Data, string edge, object node2Data)
        {
            addStaticEdge(node1Data, edge, node2Data, _staticOutEdges);
            addStaticEdge(node2Data, edge, node1Data, _staticInEdges);
        }

        protected void RemoveFromEdge(object nodeData, string removedEdge)
        {
            Dictionary<object, List<string>> edges;
            if (!_staticOutEdges.TryGetValue(nodeData, out edges))
                //there is nothing to remove
                return;

            foreach (var edgePair in edges.ToArray())
            {
                var nodeEdges = edgePair.Value;
                var targetNode = edgePair.Key;

                if (nodeEdges.Contains(removedEdge))
                {
                    nodeEdges.Remove(removedEdge);
                    _staticInEdges[targetNode][nodeData].Remove(removedEdge);
                }
            }
        }

        protected void AddGeneratedNode(NodeGenerator generator)
        {
            _generators.Add(generator);
        }

        protected void AddGeneratedEdge(NodeGenerator generator, string edge, object nodeData)
        {
            Dictionary<object, List<string>> edgeIndex;
            if (!_staticOutGeneratorEdges.TryGetValue(generator, out edgeIndex))
                _staticOutGeneratorEdges[generator] = edgeIndex = new Dictionary<object, List<string>>();

            List<string> edges;
            if (!edgeIndex.TryGetValue(nodeData, out edges))
                edgeIndex[nodeData] = edges = new List<string>();

            edges.Add(edge);
        }

        #region Generator Implementation

        protected internal override IEnumerable<string> Edges(NodeReference from, NodeReference to)
        {
            var staticGeneratorEdges = getStaticGeneratorEdges(from, to);
            var staticEdges = getStaticEdges(from, to);
            var concatenated = staticEdges.Concat(staticGeneratorEdges);

            foreach (var staticEdge in concatenated)
            {
                yield return staticEdge;
            }

            var dynamicEdges = getDynamicEdges(from, to);
            foreach (var dynamicEdge in dynamicEdges)
            {
                yield return dynamicEdge;
            }
        }

        protected internal override IEnumerable<KeyValuePair<string, NodeReference>> Incoming(NodeReference node)
        {
            //static edges
            Dictionary<object, List<string>> edgeIndex;
            if (_staticInEdges.TryGetValue(node.Data, out edgeIndex))
            {
                foreach (var indexPair in edgeIndex)
                {
                    foreach (var edge in indexPair.Value)
                    {
                        yield return new KeyValuePair<string, NodeReference>(edge, CreateReference(indexPair.Key));
                    }
                }
            }

            Dictionary<NodeGenerator, List<string>> dynamicEdgeIndex;
            if (_staticInGeneratorEdges.TryGetValue(node.Data, out dynamicEdgeIndex))
            {
                foreach (var indexPair in dynamicEdgeIndex)
                {
                    foreach (var edge in indexPair.Value)
                    {
                        foreach (var generatedNode in indexPair.Key.Generate())
                        {
                            yield return new KeyValuePair<string, NodeReference>(edge, CreateReference(generatedNode));
                        }
                    }
                }
            }

            foreach (var generator in _generators)
            {
                if (generator.HasEdges(node.Data))
                {
                    foreach (var generatedNode in generator.Generate())
                    {
                        foreach (var edge in generator.Edges(generatedNode, node.Data))
                            yield return new KeyValuePair<string, NodeReference>(edge, CreateReference(generatedNode));
                    }
                }
            }
        }

        protected internal override IEnumerable<KeyValuePair<string, NodeReference>> Outcoming(NodeReference node)
        {
            //static edges
            Dictionary<object, List<string>> edgeIndex;
            if (_staticOutEdges.TryGetValue(node.Data, out edgeIndex))
            {
                foreach (var indexPair in edgeIndex)
                {
                    foreach (var edge in indexPair.Value)
                    {
                        yield return new KeyValuePair<string, NodeReference>(edge, CreateReference(indexPair.Key));
                    }
                }
            }

            var nodeGenerator = findGenerator(node);
            if (nodeGenerator != null && _staticOutGeneratorEdges.TryGetValue(nodeGenerator, out edgeIndex))
            {
                foreach (var indexPair in edgeIndex)
                {
                    foreach (var edge in indexPair.Value)
                    {
                        yield return new KeyValuePair<string, NodeReference>(edge, CreateReference(indexPair.Key));
                    }
                }
            }
        }


        protected internal override IEnumerable<NodeReference> Outcoming(NodeReference fromNode, string edge)
        {
            //static edges
            Dictionary<object, List<string>> edgeIndex;
            if (_staticOutEdges.TryGetValue(fromNode.Data, out edgeIndex))
            {
                foreach (var indexPair in edgeIndex)
                {
                    foreach (var indexEdge in indexPair.Value)
                    {
                        if (indexEdge == edge)
                            yield return CreateReference(indexPair.Key);
                    }
                }
            }


            var nodeGenerator = findGenerator(fromNode);
            if (nodeGenerator != null)
                foreach (var target in nodeGenerator.Targets(fromNode.Data, edge))
                    yield return CreateReference(target);
        }

        protected internal override IEnumerable<NodeReference> Incoming(NodeReference toNode, string edge)
        {
            //static edges
            Dictionary<object, List<string>> edgeIndex;
            if (_staticInEdges.TryGetValue(toNode.Data, out edgeIndex))
            {
                foreach (var indexPair in edgeIndex)
                {
                    foreach (var indexEdge in indexPair.Value)
                    {
                        if (indexEdge == edge)
                            yield return CreateReference(indexPair.Key);
                    }
                }
            }

            foreach (var nodeGenerator in _generators)
                foreach (var target in nodeGenerator.Sources(toNode.Data, edge))
                    yield return CreateReference(target);
        }

        #endregion

        private void addStaticEdge(object node1Data, string edge, object node2Data, Dictionary<object, Dictionary<object, List<string>>> edgeStorage)
        {
            Dictionary<object, List<string>> edgeIndex;
            if (!edgeStorage.TryGetValue(node1Data, out edgeIndex))
                edgeStorage[node1Data] = edgeIndex = new Dictionary<object, List<string>>();

            List<string> edges;
            if (!edgeIndex.TryGetValue(node2Data, out edges))
                edgeIndex[node2Data] = edges = new List<string>();

            edges.Add(edge);
        }

        private IEnumerable<string> getStaticEdges(NodeReference from, NodeReference to)
        {
            Dictionary<object, List<string>> toIndex;
            List<string> edges;

            if (
                _staticOutEdges.TryGetValue(from.Data, out toIndex) &&
                toIndex.TryGetValue(to.Data, out edges)
                )
            {
                return edges;
            }
            else
            {
                return Enumerable.Empty<string>();
            }
        }

        private IEnumerable<string> getStaticGeneratorEdges(NodeReference from, NodeReference to)
        {
            var toGenerator = findGenerator(to);
            //TODO edges outgoing from generated nodes

            Dictionary<NodeGenerator, List<string>> toIndex;
            List<string> edges;

            if (
                toGenerator != null &&
                _staticInGeneratorEdges.TryGetValue(from.Data, out toIndex) &&
                toIndex.TryGetValue(toGenerator, out edges)
                )
            {
                return edges;
            }
            else
            {
                return Enumerable.Empty<string>();
            }
        }

        private IEnumerable<string> getDynamicEdges(NodeReference from, NodeReference to)
        {
            var generator = findGenerator(from);
            if (generator == null)
                return Enumerable.Empty<string>();

            return generator.Edges(from.Data, to.Data);
        }

        private NodeGenerator findGenerator(NodeReference node)
        {
            foreach (var generator in _generators)
            {
                if (generator.HasGenerated(node.Data))
                    return generator;
            }

            return null;
        }
    }
}
