namespace Nodum.Node
{
    public class Connection
    {
        public NodePin OutputNodePin { get; private set; }
        public NodePin InputNodePin { get; private set; }

        public Connection(NodePin outputNodePin, NodePin inputNodePin)
        {
            OutputNodePin = outputNodePin;
            InputNodePin = inputNodePin;
        }
    }
}