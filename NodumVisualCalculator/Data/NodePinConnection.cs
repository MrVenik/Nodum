using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NodumVisualCalculator.Data
{
    public class NodePinConnection
    {
        public NodePinConnection(NodePin fromPin, NodePin toPin)
        {
            FromPin = fromPin;
            ToPin = toPin;

            Line = new Line(FromPin.Position, ToPin.Position);
        }

        public NodePin FromPin { get; set; }
        public NodePin ToPin { get; set; }

        public Line Line { get; private set; }

        public void CloseConnection()
        {
            FromPin.VisualNode.OutgoingConnections.Remove(this);
            ToPin.VisualNode.IncomingConnections.Remove(this);
        }
    }
}
