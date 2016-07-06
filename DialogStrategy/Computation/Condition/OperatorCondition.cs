using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DialogStrategy.Knowledge;

namespace DialogStrategy.Computation.Condition
{
    class OperatorCondition
    {
        private readonly NodeCriterion[] _criterions;

        private readonly EdgeOperator[] _operators;

        private readonly int[] _firstOperands;

        private readonly int[] _secondOperands;


        public MatchResult Evaluate(ComposedGraph graph)
        {
            var iterators = nodeIterators(_criterions, graph);
            var operatorConfidences = new double[_operators.Length];

            bool hasError;
            do
            {
                hasError = false;
                for (var operatorIndex = 0; operatorIndex < _operators.Length; ++operatorIndex)
                {
                    var edgeOperator = _operators[operatorIndex];
                    var firstOperandIndex = _firstOperands[operatorIndex];
                    var secondOperandIndex = _secondOperands[operatorIndex];

                    var firstOperandIterator = iterators[firstOperandIndex];
                    var secondOperandIterator = iterators[secondOperandIndex];

                    var confidence = edgeOperator.StepEval(firstOperandIterator, secondOperandIterator);
                    operatorConfidences[operatorIndex] = confidence;
                    if (confidence <= 0)
                        //we has to repeat eval for evey operator - some iterator has changed
                        hasError = true;

                    if (firstOperandIterator.End || secondOperandIterator.End)
                        return null;
                }
            } while (hasError);

            var matchedNodes = from iterator in iterators select iterator.Current;

            throw new NotImplementedException();
        }

        private NodeIterator[] nodeIterators(NodeCriterion[] criterions, ComposedGraph graph)
        {
            throw new NotImplementedException();
        }
    }
}
