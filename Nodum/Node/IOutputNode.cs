namespace Nodum.Node
{
    public interface IOutputNode : INode
    {
        void AddOutgoingNode(IInputNode node);
        void RemoveOutgoingNode(IInputNode node);
        void RemoveAllOutgoingNodes();
    }
}
