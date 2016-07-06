using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DialogStrategy.Computation;
using DialogStrategy.Computation.Model;
using DialogStrategy.Computation.Condition;

namespace DialogStrategy.Learning
{
    class InformInstruction : InstructionBase
    {
        public readonly PatternNode Node;

        public InformInstruction(PatternNode node)
        {
            Node = node;
        }

        public override void Execute(Executor executor)
        {
            executor.Inform(Node);
        }

        public override string ToString()
        {
            return "Inform(" + Node + ")";
        }
    }
}
