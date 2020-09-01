using Nodum.Core;
using System;
using System.Collections.Generic;
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

        [Input] public bool ElseIfCondtition { get; set; }
        [Input] public double ElseIfTrueValue { get; set; }

        [Input] public double ElseValue { get; set; }

        [Output] public double Result { get; set; }

        public override void UpdateValue()
        {
            if (IfCondtition)
            {
                Result = IfTrueValue;
            }
            else if (ElseIfCondtition)
            {
                Result = ElseIfTrueValue;
            }
            else
            {
                Result = ElseValue;
            }
        }
    }
}
