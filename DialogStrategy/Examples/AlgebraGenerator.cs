using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DialogStrategy.Learning;
using DialogStrategy.Knowledge;
using DialogStrategy.Computation.Model;

namespace DialogStrategy.Examples
{
    class AlgebraGenerator
    {
        public static readonly string UserGoal = "UserGoal";

        public static readonly string Operation = "Operation";

        public static readonly string Addition = "Addition";

        public static readonly string Number = "Number";

        public static readonly string Multiplication = "Multiplication";

        public static readonly string Operand1 = "first operand";

        public static readonly string Operand2 = "second operand";

        public static readonly string HasResult = "has result";

        public static Graph Generate(int maxArgument)
        {
            var graph = new Graph();

            graph.Is(Operation, UserGoal);
            graph.Is(Addition, Operation);
            graph.Is(Multiplication, Operation);

            for (var arg1 = 0; arg1 < maxArgument; ++arg1)
            {
                var arg1Node = arg1.ToString();
                graph.Is(arg1Node, Number);

                for (var arg2 = 0; arg2 < maxArgument; ++arg2)
                {
                    var arg2Node = arg2.ToString();

                    var tmpAddition = arg1 + "+" + arg2;
                    var tmpMultiplication = arg1 + "*" + arg2;

                    var additionResult = (arg1 + arg2).ToString();
                    var multiplicationResult = (arg1 * arg2).ToString();

                    //numberize results
                    graph.Is(additionResult, Number);
                    graph.Is(multiplicationResult, Number);

                    //connect addition nodes
                    graph.Is(tmpAddition, Addition);
                    graph.Add(tmpAddition, Operand1, arg1Node);
                    graph.Add(tmpAddition, Operand2, arg2Node);
                    graph.Add(tmpAddition, HasResult, additionResult);

                    //connect multiplication nodes
                    graph.Is(tmpMultiplication, Multiplication);
                    graph.Add(tmpMultiplication, Operand1, arg1Node);
                    graph.Add(tmpMultiplication, Operand2, arg2Node);
                    graph.Add(tmpMultiplication, HasResult, multiplicationResult);
                }
            }

            return graph;
        }

        public static AnotatedData[] GenerateAnotatedData(int count, int maxArgument = 10)
        {
            var result = new List<AnotatedData>();

            var rnd = new Random(10);
            for (int i = 0; i < count; ++i)
            {
                var graph = Generate(maxArgument);
                var arg1 = rnd.Next(maxArgument);
                var arg2 = rnd.Next(maxArgument);

                graph.Inform(arg1.ToString(), Graph.Active);
                graph.Inform(arg2.ToString(), Graph.Active);
                graph.Inform(Addition, Graph.Active);

                var additionResult = (arg1 + arg2).ToString();
                var data = new AnotatedData(graph, new[] { new InformActionNode(graph.GetOrCreateNode(additionResult)) });

                Console.WriteLine("Generating " + arg1 + "+" + arg2);

                result.Add(data);
            }

            return result.ToArray();
        }
    }
}
