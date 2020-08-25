using Nodum.Core;
using System;

namespace Nodum.Calc
{
    [Serializable]
    public class NumberNode : Node
    {
        public override bool IsBaseNode { get; }

        [InputOutput] public double Number;

        public NumberNode(string name = "Number Node") : base(name)
        {
            IsBaseNode = true;
        }

    }
}
