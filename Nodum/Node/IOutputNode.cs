namespace Nodum.Node
{
    public interface IOutputNode : INode
    {
        NodePin OutputPin { get; }
        void AddOutgoingNode(IInputNode node);
        void RemoveOutgoingNode(IInputNode node);
        void RemoveAllOutgoingNodes();
    }
}
