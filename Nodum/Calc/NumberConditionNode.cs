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

        [Input] public bool Condtition { get; set; }
        [Input] public double IfTrue { get; set; }
        [Input] public double Else { get; set; }
        [Output] public double Result { get; set; }

        public override void UpdateValue()
        {
            Result = Condtition ? IfTrue : Else;
        }
    }
}
