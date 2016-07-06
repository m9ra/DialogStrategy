using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DialogStrategy.Knowledge;

namespace DialogStrategy.Computation.Model
{
    class InformActionNode : ActionNodeBase
    {
        public InformActionNode(Node node)
            : base(node)
        {
        }

        public override string ToString()
        {
            return "Inform(" + Node.Name + ")";
        }
    }
}
