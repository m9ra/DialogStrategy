using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DialogStrategy.Knowledge;
using DialogStrategy.Computation;
using DialogStrategy.Computation.Model;
using DialogStrategy.Computation.Condition;

namespace DialogStrategy.Examples
{
    static class Strategies
    {
        public static readonly PatternCondition PresidentOfStateCondition = new PatternConditionBuilder()
            .ActiveChild(Graphs.President).Edge(Graphs.Reigns).ActiveChild(Graphs.State)
            .Build();

        internal static IEnumerable<ActionNodeBase> PresidentOfState(Graph input)
        {
            return Computation.Model.Executor.Execute(input,
                PresidentOfStateCondition)
                    .Inform(Graphs.President)

                    .Result()
                ;
        }
    }
}
