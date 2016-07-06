using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DialogStrategy.Knowledge;

namespace DialogStrategy
{
    internal static class DialogExtensions
    {
        internal static void Inform(this Graph graph, string informedNode, params string[] flags)
        {
            foreach (var flag in flags)
            {
                graph.Add(informedNode, Graph.HasFlag, flag);
            }
        }

        internal static Graph Is(this Graph graph, string node1, string node2)
        {
            graph.Add(node1, Graph.IsRelation, node2);
            return graph;
        }
    }
}
