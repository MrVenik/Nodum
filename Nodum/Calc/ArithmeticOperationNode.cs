using Nodum.Core;
using System;
using System.Linq.Expressions;

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

        public override Expression GetExpressionForNodePin(NodePin nodePin)
        {
            if (nodePin.Node == this)
            {
                if (nodePin.Name == "Result")
                {
                    return Operation switch
                    {
                        ArithmeticOperationType.Add => Expression.Add(GetExpressionForNodePin(NodePins["InputA"]), GetExpressionForNodePin(NodePins["InputB"])),
                        ArithmeticOperationType.Subtract => Expression.Subtract(GetExpressionForNodePin(NodePins["InputA"]), GetExpressionForNodePin(NodePins["InputB"])),
                        ArithmeticOperationType.Multiply => Expression.Multiply(GetExpressionForNodePin(NodePins["InputA"]), GetExpressionForNodePin(NodePins["InputB"])),
                        ArithmeticOperationType.Divide => Expression.Divide(GetExpressionForNodePin(NodePins["InputA"]), GetExpressionForNodePin(NodePins["InputB"])),
                        ArithmeticOperationType.Remainder => Expression.Modulo(GetExpressionForNodePin(NodePins["InputA"]), GetExpressionForNodePin(NodePins["InputB"])),
                        ArithmeticOperationType.Power => Expression.Power(GetExpressionForNodePin(NodePins["InputA"]), GetExpressionForNodePin(NodePins["InputB"])),
                        ArithmeticOperationType.Root => Expression.Power(GetExpressionForNodePin(NodePins["InputA"]), Expression.Divide(Expression.Constant(1.0), GetExpressionForNodePin(NodePins["InputB"]))),
                        ArithmeticOperationType.Log => Expression.Call(typeof(Math).GetMethod("Log", new Type[] { typeof(double), typeof(double) }), GetExpressionForNodePin(NodePins["InputA"]), GetExpressionForNodePin(NodePins["InputB"])),
                        ArithmeticOperationType.Min => Expression.Call(typeof(Math).GetMethod("Min", new Type[] { typeof(double), typeof(double) }), GetExpressionForNodePin(NodePins["InputA"]), GetExpressionForNodePin(NodePins["InputB"])),
                        ArithmeticOperationType.Max => Expression.Call(typeof(Math).GetMethod("Max", new Type[] { typeof(double), typeof(double) }), GetExpressionForNodePin(NodePins["InputA"]), GetExpressionForNodePin(NodePins["InputB"])),
                        _ => throw new NotImplementedException(),
                    };
                }
            }
            return base.GetExpressionForNodePin(nodePin);
        }
    }
}
