using Nodum.Core;

namespace NodumVisualCalculator.Data
{
    public class VisualNodePin
    {
        public NodePin NodePin { get; set; }
        public VisualNode VisualNode { get; set; }
        public NodePinConnection Connection { get; set; }
        public string ElementId { get; set; }
        public string InputElementId => $"{ElementId}_Input";
        public string OutputElementId => $"{ElementId}_Output";
        public bool Showed { get; set; }
        public bool IsInternal { get; set; }

        public void ConnectToNodePin(VisualNodePin outputVisualNodePin)
        {
            NodePin.AddIncomingNodePin(outputVisualNodePin.NodePin);
            Connection?.CloseConnection();
            Connection = new NodePinConnection(outputVisualNodePin, this);
            VisualNode.IncomingConnections.Add(Connection);
            outputVisualNodePin.VisualNode.OutgoingConnections.Add(Connection);
        }
    }
}
