using Nodum.Core;
using System;

namespace Nodum.Calc
{
    [Serializable]
    public class NumberNode : Node
    {
        public override bool IsEditable => false;

        [InputOutput] public double Number;

        public NumberNode(string name = "Number Node") : base(name)
        {
        }

    }
}
