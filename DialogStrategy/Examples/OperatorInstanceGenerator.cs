using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DialogStrategy.Knowledge;

namespace DialogStrategy.Examples
{
    class OperatorInstanceGenerator : NodeGenerator
    {
        public readonly Operator Operator;

        internal OperatorInstanceGenerator(Operator op)
        {
            Operator = op;
        }

        internal override bool HasGenerated(object data)
        {
            return data is OperatorInstance;
        }

        internal override IEnumerable<object> Generate()
        {
            for (var i = 0; i < Operator.Numbers.MaxNumber; ++i)
            {
                for (var j = 0; j < i; ++j)
                {
                    yield return new OperatorInstance(i, j, Operator);
                }
            }
        }

        internal override IEnumerable<string> Edges(object fromNode, object toNode)
        {
            if (toNode as Operator == Operator)
                yield return Graph.IsRelation;

            if (toNode is int)
            {
                var toNumber = (int)toNode;
                var operatorInstance = fromNode as OperatorInstance;

                if (toNumber == operatorInstance.Operand1)
                    yield return AlgebraLayer.FirstOperandRelation;

                if (toNumber == operatorInstance.Operand2)
                    yield return AlgebraLayer.SecondOperandRelation;

                if (toNumber == operatorInstance.Result)
                    yield return AlgebraLayer.HasResultRelation;
            }
        }

        internal override bool HasEdges(object toNode)
        {
            return
                toNode is int ||
                toNode as Operator == Operator;
        }

        internal override IEnumerable<object> Targets(object fromNode, string edge)
        {
            var node = fromNode as OperatorInstance;
            if (edge == AlgebraLayer.FirstOperandRelation)
                yield return node.Operand1;

            if (edge == AlgebraLayer.SecondOperandRelation)
                yield return node.Operand2;

            if (edge == AlgebraLayer.HasResultRelation)
                yield return node.Result;
        }

        internal override IEnumerable<object> Sources(object toNode, string edge)
        {
            if (!(toNode is int))
                yield break;

            var number = (int)toNode;
            if (edge == AlgebraLayer.HasResultRelation)
            {
                for (int i = 0; i < number; ++i)
                {
                    //TODO this is addition dependant
                    yield return new OperatorInstance(i, number - i, Operator);
                }
                yield break;
            }

            if (edge == AlgebraLayer.FirstOperandRelation)
            {
                for (int i = 0; i < Operator.Numbers.MaxNumber; ++i)
                {
                    yield return new OperatorInstance(number, i, Operator);
                }
                yield break;
            }

            if (edge == AlgebraLayer.SecondOperandRelation)
            {
                for (int i = 0; i < Operator.Numbers.MaxNumber; ++i)
                {
                    yield return new OperatorInstance(i, number, Operator);
                }
                yield break;
            }
            throw new NotImplementedException();
        }
    }
}
