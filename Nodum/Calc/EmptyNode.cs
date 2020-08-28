using Nodum.Core;
using System;
using System.Runtime.Serialization;

namespace Nodum.Calc
{
    [Serializable]
    [Node(NodeCacherIgnore = true)]
    public class EmptyNode : Node
    {
        public override bool IsEditable => true;

        public EmptyNode(string name = "EmptyNode") : base(name)
        {
        }
    }
}
