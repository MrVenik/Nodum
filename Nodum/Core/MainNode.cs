using System;

namespace Nodum.Core
{
    [Serializable]
    [Node(NodeCacherIgnore = true)]
    public class MainNode : Node
    {
        public override bool IsEditable => true;

        public MainNode(string name = "MainNode") : base(name)
        {
        }
    }
}