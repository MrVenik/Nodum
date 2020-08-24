using Nodum.Core;

namespace Nodum.Calc
{
    public class NumberNode : Node
    {
        public override bool IsBaseNode => true;

        [InputOutput] public double Number;

        public NumberNode(string name = "Number Node", Node holder = null) : base(name, holder)
        {
        }
    }
}
