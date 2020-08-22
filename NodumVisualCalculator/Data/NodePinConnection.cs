using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NodumVisualCalculator.Data
{
    public class NodePinConnection
    {
        public NodePinConnection(VisualNodePin fromPin, VisualNodePin toPin)
        {
            FromPin = fromPin;
            ToPin = toPin;
        }

        public VisualNodePin FromPin { get; set; }
        public VisualNodePin ToPin { get; set; }

        public void CloseConnection()
        {
            FromPin.NodePin.RemoveOutgoingNodePin(ToPin.NodePin);
            ToPin.NodePin.RemoveIncomingNodePin(FromPin.NodePin);

            FromPin.VisualNode.OutgoingConnections.Remove(this);
            ToPin.VisualNode.IncomingConnections.Remove(this);
        }
    }
}
