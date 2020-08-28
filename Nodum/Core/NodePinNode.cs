using System;
using System.Collections.Generic;
using System.Text;

namespace Nodum.Core
{
    [Serializable]
    public class NodePinNode : Node
    {
        public NodePinNode(NodePin nodePin) : base(nodePin.Name)
        {
            ProtectedTryAddNodePin(nodePin);
        }
    }
}
