using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DialogStrategy.Examples;
using DialogStrategy.Computation.Model;
using DialogStrategy.Computation.Condition;


namespace DialogStrategy.Learning
{
    class TBL
    {
        private readonly AnotatedData[] _data;

        private readonly FunctionTransformationBase _functionTransform;

        public TBL(IEnumerable<AnotatedData> data, FunctionTransformationBase functionTransform)
        {
            _data = data.ToArray();
            _functionTransform = functionTransform;
        }

        public void Train()
        {
            var systemFunctions = new List<ConditionalFunction>();
            var nursery = new List<ConditionalFunction>();
            var rnd = new Random(1);
            nursery.Add(createEmptyFunction());

            while (true)
            {
                //try to enhance each function
                foreach (var function in systemFunctions)
                {
                    tryImprove(function);
                }

                var errors = computeErrors(systemFunctions, _data);
                var targetData = _data[rnd.Next(_data.Length)];

                var newChildren = new List<ConditionalFunction>();
                foreach (var child in nursery)
                {
                    newChildren.Add(child);
                    newChildren.AddRange(transform(child, targetData));
                }

                nursery.AddRange(newChildren);
                nurseryEvolution(nursery, systemFunctions);
            }
        }

        private ConditionalFunction createEmptyFunction()
        {
            /*
            var condition = new PatternConditionBuilder()
                .ActiveChild(AlgebraGenerator.Addition).Edge(AlgebraGenerator.Operand1).ActiveChild(AlgebraGenerator.Number, true)
                .ActiveChild(AlgebraGenerator.Addition).Edge(AlgebraGenerator.Operand2).ActiveChild(AlgebraGenerator.Number, true)
                .ActiveChild(AlgebraGenerator.Addition).Edge(AlgebraGenerator.HasResult).Node(AlgebraGenerator.Number, true).Build();

            var instruction = new InformInstruction(condition.PatternNodes.Last());
             */

            var condition = new PatternConditionBuilder()
                .Node("0")
                .ActiveChild(AlgebraGenerator.Number)                
                .Build();

            var instruction = new InformInstruction(condition.PatternNodes.Skip(1).First());

            return new ConditionalFunction(condition, new[] { instruction });
            //return new ConditionalFunction();
        }

        private void tryImprove(ConditionalFunction function)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<ConditionalFunction> transform(ConditionalFunction function, AnotatedData failingData)
        {
            return _functionTransform.GetTransformations(function, failingData);
        }

        private void nurseryEvolution(List<ConditionalFunction> nursery, List<ConditionalFunction> system)
        {
            var errors = computeErrors(system, _data);
            var ordering = new List<Tuple<int, ConditionalFunction>>();
            foreach (var child in nursery.Distinct(new Comparer()))
            {
                var improvement = errors.Count - computeErrors(new[] { child }, errors).Count;
                ordering.Add(new Tuple<int, ConditionalFunction>(improvement, child));
            }

            ordering.Sort((t1, t2) => t2.Item1 - t1.Item1);
            Console.WriteLine(ordering[0].Item1);

            nursery.Clear();
            nursery.AddRange(ordering.Take(20).Select((t) => t.Item2));
        }

        private List<AnotatedData> computeErrors(IEnumerable<ConditionalFunction> system, IEnumerable<AnotatedData> data)
        {
            var result = new List<AnotatedData>();
            foreach (var entry in data)
            {
                if (!correctAnswer(system, entry))
                    result.Add(entry);
            }

            return result;
        }

        private bool correctAnswer(IEnumerable<ConditionalFunction> system, AnotatedData entry)
        {
            foreach (var function in system)
            {
                var output = function.Evaluate(entry.Graph);
                if (output != null)
                    return entry.CheckAnotation(output);
            }

            //no response given
            return false;
        }
    }

    class Comparer : IEqualityComparer<ConditionalFunction>
    {

        public bool Equals(ConditionalFunction x, ConditionalFunction y)
        {
            return x.ToString() == y.ToString();
        }

        public int GetHashCode(ConditionalFunction obj)
        {
            return obj.ToString().GetHashCode();
        }
    }
}
