using Nodum.Core;
using System;

namespace Nodum.Calc
{
    [Serializable]
    [Node(Group = "Calc")]
    public class BooleanNotNode : Node
    {
        [Input] public double Boolean { get; set; }
        [Output] public double Not { get; set; }

        public BooleanNotNode(string name = "BooleanNotNode") : base(name)
        {
        }

        public override void UpdateValue()
        {
            Not = -Boolean;
        }
    }
}
