using Nodum.Core;
using System;

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
    }
}
