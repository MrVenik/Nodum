using Nodum.Node;

namespace NodumVisualCalculator.Data
{
    public class VisualNodePin
    {
        public NodePin NodePin { get; set; }
        public VisualNode VisualNode { get; set; }
        public Position InputPosition { get; set; }
        public Position OutputPosition { get; set; }
        public string ElementId { get; set; }
        public string InputElementId => $"{ElementId}_Input";
        public string OutputElementId => $"{ElementId}_Output";
        public bool Showed { get; set; }
        public bool IsInternal { get; set; }

        public void ConnectToNodePin(VisualNodePin outputVisualNodePin)
        {
            NodePin.AddIncomingNodePin(outputVisualNodePin.NodePin);

            NodePinConnection connection = new NodePinConnection(outputVisualNodePin, this);
            VisualNode.IncomingConnections.Add(connection);
            outputVisualNodePin.VisualNode.OutgoingConnections.Add(connection);
        }
    }
}
