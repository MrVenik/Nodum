using Nodum.Core;
using System;

namespace Nodum.Calc
{
    [Serializable]
    [Node(Group = "Calc")]
    public class ArithmeticOperationNode : Node
    {
        [Serializable]
        public enum ArithmeticOperationType
        {
            Add,
            Subtract,
            Multiply,
            Divide,
            Remainder
        }

        public override bool IsEditable => false;

        public ArithmeticOperationNode(string name = "ArithmeticOperationNode") : base(name)
        {
        }

        [NodePin(IsOption = true, IsInvokeUpdate = true, CanSetValue = true)] public ArithmeticOperationType Operation { get; set; }
        [Input] public double InputA { get; set; }
        [Input] public double InputB { get; set; }
        [Output] public double Result { get; set; }

        public override void UpdateValue()
        {
            Result = 0;
            switch (Operation)
            {
                case ArithmeticOperationType.Add:
                    Result = InputA + InputB;
                    break;
                case ArithmeticOperationType.Subtract:
                    Result = InputA - InputB;
                    break;
                case ArithmeticOperationType.Multiply:
                    Result = InputA * InputB;
                    break;
                case ArithmeticOperationType.Divide:
                    if (InputB != 0)
                    {
                        Result = InputA / InputB;
                    }
                    break;
                case ArithmeticOperationType.Remainder:
                    if (InputB != 0)
                    {
                        Result = InputA % InputB;
                    }
                    break;
            }
        }
    }
}
