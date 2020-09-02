﻿using Nodum.Core;
using System;

namespace Nodum.Calc
{
    [Serializable]
    [Node(Group = "Calc")]
    public class BooleanNotNode : Node
    {
        [Input] public bool Boolean { get; set; }
        [Output] public bool Not { get; set; }

        public BooleanNotNode(string name = "BooleanNotNode") : base(name)
        {
        }

        public override void UpdateValue()
        {
            Not = !Boolean;
        }

        public override string GetStringForNodePin(NodePin nodePin)
        {
            if (nodePin.Node == this)
            {
                if (nodePin.Name == "Not")
                {
                    return $"!{GetStringForNodePin(NodePins["Boolean"])}";
                }
            }
            return base.GetStringForNodePin(nodePin);
        }
    }
}
