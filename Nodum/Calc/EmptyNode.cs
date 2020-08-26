using Nodum.Core;
using System;

namespace Nodum.Calc
{
    [Serializable]
    [Node]
    public class EmptyNode : Node
    {
        public override bool IsEditable => true;

        public EmptyNode(string name = "Empty Node") : base(name)
        {
        }
    }
}
