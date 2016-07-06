using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DialogStrategy.Knowledge
{
    abstract class NodeGenerator
    {
        abstract internal bool HasGenerated(object data);

        abstract internal IEnumerable<object> Generate();

        abstract internal IEnumerable<string> Edges(object fromNode, object toNode);

        abstract internal bool HasEdges(object toNode);

        abstract internal IEnumerable<object> Targets(object fromNode, string edge);

        abstract internal IEnumerable<object> Sources(object toNode, string edge);
    }
}
