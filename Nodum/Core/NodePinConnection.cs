using System;
using System.Collections.Generic;
using System.Text;

namespace Nodum.Core
{
    [Serializable]
    public class NodePinConnection
    {
        public NodePinConnection(NodePin fromPin, NodePin toPin)
        {
            FromPin = fromPin;
            ToPin = toPin;
        }

        public NodePin FromPin { get; }
        public NodePin ToPin { get; }

        public void CloseConnection()
        {
            ToPin.RemoveIncomingNodePin(FromPin);

            FromPin.Node.OutgoingConnections.Remove(this);
            ToPin.Node.IncomingConnections.Remove(this);
        }
    }
}
