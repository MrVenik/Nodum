﻿namespace Nodum.Node
{
    public interface ICommutateInputNode : IInputNode
    {
        CommutateInputNodePin InputPin { get; }

        void AddIncomingNode(IOutputNode nodePin);
        void AddIncomingNode(IOutputNode nodePin, int index);
        void RemoveIncomingNode(IOutputNode nodePin);
        void RemoveIncomingNode(int index);
        void RemoveAllIncomingNodes();
    }
}