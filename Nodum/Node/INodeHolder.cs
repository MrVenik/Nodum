using System.Collections.Generic;

namespace Nodum.Node
{
    public interface INodeHolder : INode
    {
        List<INode> Nodes { get; }
        void AddNode(INode node);
        void AddNodes(params INode[] nodes);
        void RemoveNode(INode nodePin);
        void RemoveAllNodes();
    }
}