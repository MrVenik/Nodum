using Nodum.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nodum.Calc
{
    [Serializable]
    [Node(Group = "Calc")]
    public class CompareNumberNode : Node
    {
        [Serializable]
        public enum CompareOperationType
        {
            Equals,
            NotEquals,
            LessThan,
            GreaterThan,
            LessThanOrEqual,
            GreaterThanOrEqual
        }

        [NodePin(IsOption = true, IsInvokeUpdate = true, CanSetValue = true)] public CompareOperationType Operation { get; set; }
        [Input] public double InputA { get; set; }
        [Input] public double InputB { get; set; }
        [Output] public bool Result { get; set; }

        public CompareNumberNode(string name = "CompareNumberNode") : base(name)
        {
        }

        public override void UpdateValue()
        {
            Result = false;

            switch (Operation)
            {
                case CompareOperationType.Equals:
                    Result = (InputA == InputB);
                    break;
                case CompareOperationType.NotEquals:
                    Result = (InputA != InputB);
                    break;
                case CompareOperationType.LessThan:
                    Result = (InputA < InputB);
                    break;
                case CompareOperationType.GreaterThan:
                    Result = (InputA > InputB);
                    break;
                case CompareOperationType.LessThanOrEqual:
                    Result = (InputA <= InputB);
                    break;
                case CompareOperationType.GreaterThanOrEqual:
                    Result = (InputA >= InputB);
                    break;
                default:
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
                        CompareOperationType.Equals => $"({GetStringForNodePin(NodePins["InputA"])} == {GetStringForNodePin(NodePins["InputB"])})",
                        CompareOperationType.NotEquals => $"({GetStringForNodePin(NodePins["InputA"])} != {GetStringForNodePin(NodePins["InputB"])})",
                        CompareOperationType.LessThan => $"({GetStringForNodePin(NodePins["InputA"])} < {GetStringForNodePin(NodePins["InputB"])})",
                        CompareOperationType.GreaterThan => $"({GetStringForNodePin(NodePins["InputA"])} > {GetStringForNodePin(NodePins["InputB"])})",
                        CompareOperationType.LessThanOrEqual => $"({GetStringForNodePin(NodePins["InputA"])} <= {GetStringForNodePin(NodePins["InputB"])})",
                        CompareOperationType.GreaterThanOrEqual => $"({GetStringForNodePin(NodePins["InputA"])} >= {GetStringForNodePin(NodePins["InputB"])})",
                        _ => $"({GetStringForNodePin(NodePins["InputA"])} == {GetStringForNodePin(NodePins["InputB"])})",
                    };
                }
            }
            return base.GetStringForNodePin(nodePin);
        }
    }
}
