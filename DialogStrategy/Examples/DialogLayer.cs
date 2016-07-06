using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DialogStrategy.Knowledge;

namespace DialogStrategy.Examples
{
    class DialogLayer : GeneratedLayer
    {
        public void Inform(NodeReference node)
        {
            SetEdge(node.Data, Graph.HasFlag, Graph.Active);            
        }

        internal NodeReference GetNode(string wordRepresentation)
        {
            int value;
            if (int.TryParse(wordRepresentation, out value))
                return CreateReference(value);

            return CreateReference(wordRepresentation);
        }
    }
}
