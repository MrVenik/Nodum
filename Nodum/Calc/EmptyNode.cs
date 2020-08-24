using Nodum.Core;
using System;

namespace Nodum.Calc
{
    [Serializable]
    public class EmptyNode : Node
    {
        public override bool IsBaseNode => false;

        public EmptyNode(string name = "Empty Node", Node holder = null) : base(name, holder)
        {
        }
    }
}
