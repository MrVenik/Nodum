using Nodum.Core;
using System;

namespace Nodum.Calc
{
    [Serializable]
    [Node(Group = "Calc")]
    public class BooleanNode : Node
    {
        public override bool IsEditable => false;

        [InputOutput] public bool Boolean { get; set; }

        public BooleanNode(string name = "BooleanNode") : base(name)
        {
        }
    }
}
