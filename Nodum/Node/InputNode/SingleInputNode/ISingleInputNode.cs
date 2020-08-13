namespace Nodum.Node
{
    public interface ISingleInputNode : IInputNode
    {
        InputNodePin InputPin { get; }

        void AddIncomingNode(IOutputNode nodePin);
        void RemoveIncomingNode();
    }
}