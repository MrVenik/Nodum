using Nodum.Core;
using System;
using System.Linq.Expressions;

namespace Nodum.Calc
{
    [Serializable]
    [Node(Group = "Calc")]
    public class NumberNegateNode : Node
    {
        public override bool IsEditable => false;

        [Input] public double Number { get; set; }
        [Output] public double Negate { get; set; }

        public NumberNegateNode(string name = "NumberNegateNode") : base(name)
        {
        }

        public override void UpdateValue()
        {
            Negate = -Number;
        }

        public override string GetStringForNodePin(NodePin nodePin)
        {
            if (nodePin.Node == this)
            {
                if (nodePin.Name == "Negate")
                {
                    return $"-{GetStringForNodePin(NodePins["Number"])}";
                }
            }
            return base.GetStringForNodePin(nodePin);
        }

        public override Expression GetExpressionForNodePin(NodePin nodePin)
        {
            if (nodePin.Node == this)
            {
                if (nodePin.Name == "Negate")
                {
                    return Expression.Negate(GetExpressionForNodePin(NodePins["Number"]));
                }
            }
            return base.GetExpressionForNodePin(nodePin);
        }
    }
}
