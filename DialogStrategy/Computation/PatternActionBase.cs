using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DialogStrategy.Knowledge;

namespace DialogStrategy.Computation
{
    abstract class PatternActionBase
    {
        public abstract void Apply(ActionContext context, NodesSubstitution substitution, double factor);
    }
}
