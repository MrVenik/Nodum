using Nodum.Core;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Nodum.Calc
{
    [Serializable]
    [Node(Group = "Calc")]
    public class NumberConditionNode : Node
    {
        public override bool IsEditable => false;

        public NumberConditionNode(string name = "NumberConditionNode") : base(name)
        {
        }

        [Input] public bool IfCondtition { get; set; }
        [Input] public double IfTrueValue { get; set; }

        [Input] public double ElseValue { get; set; }

        [Output] public double Result { get; set; }

        public override void UpdateValue()
        {
            if (IfCondtition)
            {
                Result = IfTrueValue;
            }
            else
            {
                Result = ElseValue;
            }
        }

        public override string GetStringForNodePin(NodePin nodePin)
        {
            if (nodePin.Node == this)
            {
                if (nodePin.Name == "Result")
                {
                    return $"({GetStringForNodePin(NodePins["IfCondtition"])} ? {GetStringForNodePin(NodePins["IfTrueValue"])} : {GetStringForNodePin(NodePins["ElseValue"])}))";
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
                    return Expression.IfThenElse(GetExpressionForNodePin(NodePins["IfCondtition"]), GetExpressionForNodePin(NodePins["IfTrueValue"]), GetExpressionForNodePin(NodePins["ElseValue"]));
                }
            }
            return base.GetExpressionForNodePin(nodePin);
        }
    }
}
