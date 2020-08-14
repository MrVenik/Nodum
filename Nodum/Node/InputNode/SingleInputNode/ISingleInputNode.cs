namespace Nodum.Node
{
    public interface ISingleInputNode : IInputNode
    {
        IOutputNode IncomingNode { get; }

        void AddIncomingNode(IOutputNode nodePin);
        void RemoveIncomingNode();
    }
}