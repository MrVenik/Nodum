using Nodum.Core;

namespace Nodum.Calc
{
    public class EmptyNode : Node
    {
        public override bool IsBaseNode => false;

        public EmptyNode(string name = "Empty Node", Node holder = null) : base(name, holder)
        {
        }
    }
}
