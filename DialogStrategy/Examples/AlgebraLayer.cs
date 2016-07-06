using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DialogStrategy.Knowledge;

namespace DialogStrategy.Examples
{
    class AlgebraLayer : GeneratedLayer
    {
        public const string NumberParent = "number";

        public const string OperatorParent = "operator";

        public const string FirstOperandRelation = "first operand";

        public const string SecondOperandRelation = "second operand";

        public const string HasResultRelation = "has result";

        public const string FollowsRelation = "follows";

        public readonly Operator AddingOperator;

        public readonly NumberGenerator Numbers;

        public AlgebraLayer(int maxNumber)
        {
            Numbers = new NumberGenerator(maxNumber);
            AddingOperator = new Operator(Numbers, "+", (o1, o2) => o1 + o2);

            //numbers
            AddNode(NumberParent);
            AddGeneratedNode(Numbers);

            AddGeneratedEdge(Numbers, Graph.IsRelation, NumberParent);

            //operators
            AddNode(OperatorParent);
            AddNode(AddingOperator);
            AddGeneratedNode(AddingOperator.InstanceGenerator);

            SetEdge(AddingOperator, Graph.IsRelation, OperatorParent);
            AddGeneratedEdge(AddingOperator.InstanceGenerator, Graph.IsRelation, AddingOperator);
        }
    }

    class Operator
    {
        public readonly NumberGenerator Numbers;

        public readonly string Representation;

        public readonly Func<int, int, int> Executor;

        public readonly NodeGenerator InstanceGenerator;
        
        public Operator(NumberGenerator numbers,string representation, Func<int, int, int> executor)
        {
            this.Representation = representation;
            this.Executor = executor;
            this.InstanceGenerator = new OperatorInstanceGenerator(this);
            this.Numbers = numbers;
        }

        public int Compute(int op1, int op2)
        {
            return Executor(op1, op2);
        }

        public override string ToString()
        {
            return Representation;
        }
    }

    class OperatorInstance
    {
        public readonly int Operand1;
        public readonly int Operand2;
        public readonly int Result;
        public readonly Operator Operator;

        public OperatorInstance(int operand1, int operand2, Operator op)
        {
            Operand1 = operand1;
            Operand2 = operand2;
            Operator = op;
            Result = op.Executor(Operand1, Operand2);

        }

        public override string ToString()
        {
            return Operand1 + Operator.ToString() + Operand2;
        }

        public override int GetHashCode()
        {
            return Operand1 + Operand2 + Result + Operator.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var o = obj as OperatorInstance;
            if (o == null)
                return false;

            return
                Operand1 == o.Operand1 &&
                Operand2 == o.Operand2 &&
                Operator == o.Operator;
        }
    }

}
