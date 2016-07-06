using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DialogStrategy.Computation.Model;

namespace DialogStrategy.Learning
{
    abstract class InstructionBase
    {
        public abstract void Execute(Executor executor);
    }
}
