namespace Nodum.Node
{
    public interface IMultipleInputNode : IInputNode
    {
        IOutputNode[] IncomingNodes { get; }
        void AddIncomingNode(IOutputNode nodePin, int index);
        void RemoveIncomingNode(IOutputNode nodePin);
        void RemoveIncomingNode(int index);
        void RemoveAllIncomingNodes();
    }
}