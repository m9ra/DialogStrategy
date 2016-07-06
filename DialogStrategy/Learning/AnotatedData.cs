using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DialogStrategy.Knowledge;
using DialogStrategy.Computation.Model;

namespace DialogStrategy.Learning
{
    class AnotatedData
    {
        public readonly Graph Graph;

        public readonly IEnumerable<ActionNodeBase> Anotation;

        public AnotatedData(Graph graph, IEnumerable<ActionNodeBase> anotation)
        {
            this.Graph = graph;
            this.Anotation = anotation.ToArray();
        }

        internal IEnumerable<InformActionNode> GetInformed()
        {
            var result = new List<InformActionNode>();
            foreach (var action in Anotation)
            {
                if (action is InformActionNode)
                    result.Add(action as InformActionNode);
            }

            return result;
        }

        internal bool CheckAnotation(IEnumerable<ActionNodeBase> outputNodes)
        {
            //TODO correct comparing
            foreach (var desired in Anotation)
            {
                var contains = false;
                foreach (var output in outputNodes)
                {
                    if (output.Node == desired.Node)
                        contains = true;
                }

                if (!contains)
                    return false;
            }

            return Anotation.Count() == outputNodes.Count();
        }
    }
}
