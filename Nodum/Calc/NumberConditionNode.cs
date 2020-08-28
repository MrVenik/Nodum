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
        [Input] public bool InputA { get; set; }
        [Input] public bool InputB { get; set; }
        [Output] public bool Result { get; set; }

        public override void UpdateValue()
        {
            if (Condtition)
            {
                Result = InputA;
            }
            else
            {
                Result = InputB;
            }
        }
    }
}
