﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogStrategy.Knowledge
{
    public abstract class GraphLayerBase
    {
        abstract internal protected IEnumerable<string> Edges(NodeReference from, NodeReference to);

        abstract internal protected IEnumerable<KeyValuePair<string, NodeReference>> Incoming(NodeReference node);

        abstract internal protected IEnumerable<KeyValuePair<string, NodeReference>> Outcoming(NodeReference node);
        
        abstract internal protected IEnumerable<NodeReference> Outcoming(NodeReference fromNode, string edge);

        abstract internal protected IEnumerable<NodeReference> Incoming(NodeReference toNode, string edge);

        protected NodeReference CreateReference(object data)
        {
            return new NodeReference(data);
        }

    }
}
