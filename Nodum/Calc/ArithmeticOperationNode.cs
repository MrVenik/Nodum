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
            Remainder,
            Power,
            Root,
            Log,
            Min,
            Max
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
                case ArithmeticOperationType.Power:
                    Result = Math.Pow(InputA, InputB);
                    break;
                case ArithmeticOperationType.Root:
                    Result = Math.Pow(InputA, 1.0 / InputB);
                    break;
                case ArithmeticOperationType.Log:
                    if (InputB > 0)
                    {
                        Result = Math.Log(InputA, InputB);
                    }
                    else
                    {
                        Result = Math.Log(InputA);
                    }
                    break;
                case ArithmeticOperationType.Min:
                    Result = Math.Min(InputA, InputB);
                    break;
                case ArithmeticOperationType.Max:
                    Result = Math.Max(InputA, InputB);
                    break;
            }
        }

        public override string GetStringForNodePin(NodePin nodePin)
        {
            if (nodePin.Node == this)
            {
                if (nodePin.Name == "Result")
                {
                    return Operation switch
                    {
                        ArithmeticOperationType.Add => $"({GetStringForNodePin(NodePins["InputA"])} + {GetStringForNodePin(NodePins["InputB"])})",
                        ArithmeticOperationType.Subtract => $"({GetStringForNodePin(NodePins["InputA"])} - {GetStringForNodePin(NodePins["InputB"])})",
                        ArithmeticOperationType.Multiply => $"({ GetStringForNodePin(NodePins["InputA"])} * { GetStringForNodePin(NodePins["InputB"])} )",
                        ArithmeticOperationType.Divide => $"({ GetStringForNodePin(NodePins["InputA"])} / { GetStringForNodePin(NodePins["InputB"])} )",
                        ArithmeticOperationType.Remainder => $"({ GetStringForNodePin(NodePins["InputA"])} % { GetStringForNodePin(NodePins["InputB"])})",
                        ArithmeticOperationType.Power => $"System.Math.Pow({GetStringForNodePin(NodePins["InputA"])}, {GetStringForNodePin(NodePins["InputB"])})",
                        ArithmeticOperationType.Root => $"System.Math.Pow({GetStringForNodePin(NodePins["InputA"])}, 1.0 / {GetStringForNodePin(NodePins["InputB"])})",
                        ArithmeticOperationType.Log => $"System.Math.Log({GetStringForNodePin(NodePins["InputA"])}, {GetStringForNodePin(NodePins["InputB"])})",
                        ArithmeticOperationType.Min => $"System.Math.Min({GetStringForNodePin(NodePins["InputA"])}, {GetStringForNodePin(NodePins["InputB"])})",
                        ArithmeticOperationType.Max => $"System.Math.Max({GetStringForNodePin(NodePins["InputA"])}, {GetStringForNodePin(NodePins["InputB"])})",
                        _ => $"{GetStringForNodePin(NodePins["InputA"])} + {GetStringForNodePin(NodePins["InputB"])}",
                    };
                }                             
            }
            return base.GetStringForNodePin(nodePin);
        }
    }
}
