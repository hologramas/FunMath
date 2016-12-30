//
// MIT License.
//

using System;
using System.Collections.Generic;

namespace ElementaryArithmetics
{
    /// <summary>
    /// Generates Arithmetic operations
    /// </summary>
    /// <remarks>
    /// The generator uses the MaxTotal property to limit the maximum number in any operation it generates.
    /// The generator will use all posible operands and factors up to the given total in a random order. Once there are no more factors, the generator starts over with a fresh random shuffle of all possible ones.
    /// </remarks>
    public class ArithmeticOperationGenerator
    {
        private Random rand = new Random();
        private List<Operator> operators;
        private IEnumerator<int> operandValueEnumerator;
        private IEnumerator<int> mulFactorEnumerator;
        private int currentMulOperand;

        public ArithmeticOperationGenerator(int maxTotal, IEnumerable<Operator> operators)
        {
            if (maxTotal <= 0)
            {
                throw new ArgumentOutOfRangeException("maxTotal", "maxTotal must be a positive number greater than 0.");
            }

            this.MaxTotal = maxTotal;
            this.operators = new List<Operator>(operators);
            this.operandValueEnumerator = this.GetOperandValues().GetEnumerator();
        }

        public int MaxTotal { get; private set; }

        public ArithmeticOperation CreateOperation()
        {
            Operator oper = this.operators[rand.Next(this.operators.Count)];
            int leftOperand;
            int rightOperand;
            int anOperand;
            switch (oper)
            {
                case Operator.Addition:
                    anOperand = this.GetNextOperandValue();
                    if (rand.Next(2) == 0)
                    {
                        leftOperand = anOperand;
                        rightOperand = this.rand.Next(this.MaxTotal - leftOperand + 1);
                    }
                    else
                    {
                        rightOperand = anOperand;
                        leftOperand = this.rand.Next(this.MaxTotal - rightOperand + 1);
                    }
                    break;

                case Operator.Subtraction:
                    anOperand = this.GetNextOperandValue();
                    switch(rand.Next(3))
                    {
                        case 0:
                            leftOperand = anOperand;
                            rightOperand = this.rand.Next(leftOperand + 1);
                            break;
                        case 1:
                            rightOperand = anOperand;
                            leftOperand = this.rand.Next(rightOperand, this.MaxTotal + 1);
                            break;
                        case 2:
                        default:
                            leftOperand = this.rand.Next(anOperand, this.MaxTotal + 1);
                            rightOperand = leftOperand - anOperand;
                            break;
                    }
                    break;

                case Operator.Multiplication:
                case Operator.Division:
                    if ((this.mulFactorEnumerator == null) || (!this.mulFactorEnumerator.MoveNext()))
                    {
                        this.currentMulOperand  = this.GetNextOperandValue();
                        if(this.currentMulOperand == 0)
                        {
                            this.currentMulOperand = this.MaxTotal;
                        }
                        this.mulFactorEnumerator = this.GetFactorValues(this.currentMulOperand).GetEnumerator();
                        this.mulFactorEnumerator.MoveNext();
                    }

                    if (oper == Operator.Multiplication)
                    {
                        leftOperand = this.mulFactorEnumerator.Current;
                        rightOperand = (this.currentMulOperand / this.mulFactorEnumerator.Current);
                    }
                    else
                    {
                        leftOperand = this.currentMulOperand;
                        rightOperand = ((this.rand.Next(0, 2) == 0) ? this.mulFactorEnumerator.Current : (this.currentMulOperand / this.mulFactorEnumerator.Current));
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }

            return new ArithmeticOperation(leftOperand, rightOperand, oper);
        }

        private int GetNextOperandValue()
        {
            if (!this.operandValueEnumerator.MoveNext())
            {
                this.operandValueEnumerator = this.GetOperandValues().GetEnumerator();
                this.operandValueEnumerator.MoveNext();
            }

            return this.operandValueEnumerator.Current;
        }

        private IEnumerable<int> GetOperandValues()
        {
            var record = new Dictionary<int, int>();
            for (int i = 0; i <= this.MaxTotal; i++)
            {
                int key = this.rand.Next(i, this.MaxTotal + 1);
                int valueInIndex = (record.ContainsKey(i) ? record[i] : i);

                int nextValue;
                if (record.ContainsKey(key))
                {
                    nextValue = record[key];
                    record[key] = valueInIndex;
                }
                else
                {
                    nextValue = key;
                    record.Add(key, valueInIndex);
                }

                yield return nextValue;
            }
        }

        private IEnumerable<int> GetFactorValues(int total)
        {
            var factorSet = new HashSet<int>();
            float rationalTotal = (float)total;
            for (int i = 1; i <= total; i++)
            {
                float div = (rationalTotal / i);
                if ((div - (int)div) == 0)
                {
                    if (factorSet.Contains(i) || factorSet.Contains((int)div))
                    {
                        break;
                    }
                    factorSet.Add(i);
                }

                if (div < 2.0f)
                {
                    break;
                }
            }

            int[] factors = new int[factorSet.Count];
            factorSet.CopyTo(factors);
            for (int i = 0; i < factors.Length; i++)
            {
                int pickedIndex = this.rand.Next(i, factors.Length);
                int pickedFactor = factors[pickedIndex];
                factors[pickedIndex] = factors[i];
                yield return pickedFactor;
            }
        }
    }
}
