using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DialogStrategy.Knowledge;

namespace DialogStrategy.Computation.Model
{
    abstract class ActionNodeBase
    {
        public readonly Node Node;

        protected ActionNodeBase(Node node)
        {
            this.Node = node;
        }
    }
}
