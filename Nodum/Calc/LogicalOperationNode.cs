using Nodum.Core;
using System;
using System.Linq.Expressions;

namespace Nodum.Calc
{
    [Serializable]
    [Node(Group = "Calc")]
    public class LogicalOperationNode : Node
    {
        [Serializable]
        public enum LogicalOperationType
        {
            And,
            Or
        }

        public override bool IsEditable => false;

        public LogicalOperationNode(string name = "LogicalOperationNode") : base(name)
        {
        }

        [NodePin(IsOption = true, IsInvokeUpdate = true, CanSetValue = true)] public LogicalOperationType Operation { get; set; }
        [Input] public bool InputA { get; set; }
        [Input] public bool InputB { get; set; }
        [Output] public bool Result { get; set; }

        public override void UpdateValue()
        {
            Result = true;

            switch (Operation)
            {
                case LogicalOperationType.And:
                    Result = InputA && InputB;
                    break;
                case LogicalOperationType.Or:
                    Result = InputA || InputB;
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
                        LogicalOperationType.And => $"({GetStringForNodePin(NodePins["InputA"])} && {GetStringForNodePin(NodePins["InputB"])})",
                        LogicalOperationType.Or => $"({GetStringForNodePin(NodePins["InputA"])} || {GetStringForNodePin(NodePins["InputB"])})",
                        _ => $"({GetStringForNodePin(NodePins["InputA"])} && {GetStringForNodePin(NodePins["InputB"])})",
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
                        LogicalOperationType.And => Expression.And(GetExpressionForNodePin(NodePins["InputA"]), GetExpressionForNodePin(NodePins["InputB"])),
                        LogicalOperationType.Or => Expression.Or(GetExpressionForNodePin(NodePins["InputA"]), GetExpressionForNodePin(NodePins["InputB"])),
                        _ => throw new NotImplementedException(),
                    };
                }
            }
            return base.GetExpressionForNodePin(nodePin);
        }
    }
}
