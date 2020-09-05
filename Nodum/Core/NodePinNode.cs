using System;
using System.Collections.Generic;
using System.Text;

namespace Nodum.Core
{
    [Serializable]
    public class NodePinNode : Node
    {
        [NonSerialized]
        private NodePin _nodePin;

        public override bool IsEditable => false;

        public NodePinNode(NodePin nodePin) : base(nodePin.Name)
        {
            _nodePin = nodePin;
            ProtectedTryAddNodePin(nodePin);
        }

        public override void Close()
        {
            Holder?.RemoveNodePin(_nodePin);

            base.Close();
        }
    }
}
