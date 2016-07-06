using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DialogStrategy.Knowledge;

namespace DialogStrategy.Computation
{
    abstract class ConditionBase
    {
        public abstract double Check(Graph input);
    }
}
