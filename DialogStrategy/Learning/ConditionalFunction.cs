using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DialogStrategy.Knowledge;
using DialogStrategy.Computation.Model;
using DialogStrategy.Computation.Condition;

namespace DialogStrategy.Learning
{
    class ConditionalFunction
    {
        public readonly PatternCondition Condition;

        public readonly IEnumerable<InstructionBase> Instructions;

        /// <summary>
        /// Empty function that does not nothing
        /// </summary>
        public ConditionalFunction()
        {
            Condition = new PatternCondition(new PatternNode[0]);
            Instructions = new InstructionBase[0];
        }

        public ConditionalFunction(PatternCondition condition, IEnumerable<InstructionBase> instructions)
        {
            Condition = condition;
            Instructions = instructions.ToArray();
        }

        internal IEnumerable<ActionNodeBase> Evaluate(Graph graph)
        {
            if (Condition.Check(graph) > 0)
            {
                var executor = new Executor(Condition.GetLastEvaluation());

                foreach (var instruction in Instructions)
                {
                    instruction.Execute(executor);
                }
                return executor.Result();
            }
            else
            {
                return null;
            }
        }

        public override string ToString()
        {
            return Condition.ToString() + " --> " + string.Join("; ", Instructions);
            
        }
    }
}
