namespace Nodum.Node
{
    public interface IMultipleInputNode : IInputNode
    {
        InputNodePin[] InputPins { get; }
        void AddIncomingNode(IOutputNode nodePin, int index);
        void RemoveIncomingNode(IOutputNode nodePin);
        void RemoveIncomingNode(int index);
        void RemoveAllIncomingNodes();
    }
}