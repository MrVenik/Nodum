﻿using Nodum.Core;
using System;

namespace Nodum.Calc
{
    [Serializable]
    [Node(Group = "Calc")]
    public class NumberNode : Node
    {
        public override bool IsEditable => false;

        [InputOutput] public double Number { get; set; }

        public NumberNode(string name = "NumberNode") : base(name)
        {
        }

    }
}
