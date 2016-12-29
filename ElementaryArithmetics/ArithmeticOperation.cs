//
//
//

using System.Runtime.Serialization;

namespace ElementaryArithmetics
{
    [DataContract(Namespace ="")]
    public enum Operator
    {
        [EnumMember(Value ="Addition")]
        Addition = 0,
        [EnumMember(Value ="Subtraction")]
        Subtraction = 1,
        [EnumMember(Value ="Multiplication")]
        Multiplication = 2,
        [EnumMember(Value ="Division")]
        Division = 3
    }

    [DataContract(Namespace = "")]
    public enum ProblemType
    {
        [EnumMember(Value ="FindRightOperand")]
        FindRightOperand = 0,
        [EnumMember(Value ="FindLeftOperand")]
        FindLeftOperand = 1,
        [EnumMember(Value ="FindTotal")]
        FindTotal = 2
    }

    public interface IOperation
    {
        int Resolve();
    }

    public class NumericOperation :IOperation
    {
        public NumericOperation(int number)
        {
            this.Number = number;
        }

        public int Number { get; set; }
        
        public int Resolve()
        {
            return this.Number;
        }
    }

    public class ArithmeticOperation : IOperation
    {
        public ArithmeticOperation(IOperation leftOperand, IOperation rightOpeand, Operator oper)
        {
            this.LeftOperand = leftOperand;
            this.RightOperand = rightOpeand;
            this.Operator = oper;
        }
  
        public ArithmeticOperation(int leftOperand, int rightOperand, Operator oper)
        {
            this.LeftOperand = new NumericOperation(leftOperand);
            this.RightOperand = new NumericOperation(rightOperand);
            this.Operator = oper;
        }

        public IOperation LeftOperand { get; set; }
        public IOperation RightOperand { get; set; }
        public Operator Operator { get; set; }

        public int Resolve()
        {
            int result = 0;
            switch (this.Operator)
            {
                case Operator.Addition:
                    result = (this.LeftOperand.Resolve() + this.RightOperand.Resolve());
                    break;

                case Operator.Subtraction:
                    result = (this.LeftOperand.Resolve() - this.RightOperand.Resolve());
                    break;

                case Operator.Multiplication:
                    result = (this.LeftOperand.Resolve() * this.RightOperand.Resolve());
                    break;

                case Operator.Division:
                    result = (this.LeftOperand.Resolve() / this.RightOperand.Resolve());
                    break;
            }
            return result;
        }
    }
}
