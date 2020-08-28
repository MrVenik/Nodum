﻿using Nodum.Core;
using System;

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
    }
}
