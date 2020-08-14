using System.Collections.Generic;

namespace Nodum.Node
{
    public class CommutateInputValueNode<T> : ValueNode<T>, ICommutateInputNode
    {
        public int AmountOfInputs { get => 1; }

        public List<IOutputNode> IncomingNodes { get; private set; } = new List<IOutputNode>();

        public CommutateInputValueNode(string name, T value = default)
            : base(name, value)
        {
        }

        public void AddIncomingNode(IOutputNode outputNode)
        {
            if (!IncomingNodes.Contains(outputNode) && outputNode != this && CanConnectTo(outputNode))
            {
                outputNode.AddOutgoingNode(this);

                IncomingNodes.Add(outputNode);

                UpdateValue();
            }

        }

        public void AddIncomingNode(IOutputNode outputNode, int index)
        {
            if (index < IncomingNodes.Count)
            {
                if (outputNode != this && CanConnectTo(outputNode))
                {
                    IncomingNodes[index]?.RemoveOutgoingNode(this);
                    IncomingNodes[index] = outputNode;
                    IncomingNodes[index].AddOutgoingNode(this);
                    UpdateValue();
                }
            }
        }

        public void RemoveAllIncomingNodes()
        {
            IncomingNodes.Clear();

            UpdateValue();
        }

        public void RemoveIncomingNode(IOutputNode outputNode)
        {
            IncomingNodes.RemoveAll(p => p == outputNode);

            UpdateValue();
        }

        public void RemoveIncomingNode(int index)
        {
            IncomingNodes.RemoveAt(index);

            UpdateValue();
        }

        public override void Close()
        {
            base.Close();

            RemoveAllIncomingNodes();
        }
    }
}