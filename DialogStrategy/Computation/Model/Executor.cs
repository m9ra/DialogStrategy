using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DialogStrategy.Knowledge;
using DialogStrategy.Computation.Condition;

namespace DialogStrategy.Computation.Model
{
    class Executor
    {
        private readonly List<ActionNodeBase> nodes = new List<ActionNodeBase>();

        private readonly Dictionary<PatternNode, Node> evaluation;

        public Executor(Dictionary<PatternNode, Node> evaluation)
        {
            this.evaluation = evaluation;
        }

        public static Executor Execute(Graph graph, PatternCondition condition)
        {
            var confidence = condition.Check(graph);
            var evaluation = condition.GetLastEvaluation();

            return new Executor(evaluation);
        }

        internal Executor Inform(PatternNode node)
        {
            nodes.Add(new InformActionNode(evaluation[node]));

            return this;
        }

        internal Executor Inform(string name)
        {
            foreach (var node in evaluation.Keys)
            {
                if (node.Name == name)
                    return Inform(node);
            }

            return null;
        }

        internal IEnumerable<ActionNodeBase> Result()
        {
            return nodes;
        }
    }
}
