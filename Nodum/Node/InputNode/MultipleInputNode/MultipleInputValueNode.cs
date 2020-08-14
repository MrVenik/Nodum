using System.Collections.Generic;
using System.Linq;

namespace Nodum.Node
{
    public class MultipleInputValueNode<T> : ValueNode<T>, IMultipleInputNode
    {
        public int AmountOfInputs { get => IncomingNodes.Length; }
        public IOutputNode[] IncomingNodes { get; private set; }

        public MultipleInputValueNode(string name, int amountOfInputs, T value = default)
            : base(name, value)
        {
            IncomingNodes = new IOutputNode[amountOfInputs];
        }

        public void AddIncomingNode(IOutputNode outputNode, int index)
        {
            if (index < IncomingNodes.Length && outputNode != this && CanConnectTo(outputNode))
            {
                IncomingNodes[index]?.RemoveOutgoingNode(this);
                IncomingNodes[index] = outputNode;
                IncomingNodes[index].AddOutgoingNode(this);
                UpdateValue();
            }
        }

        public void RemoveAllIncomingNodes()
        {
            for (int i = 0; i < IncomingNodes.Length; i++)
            {
                RemoveIncomingNode(i);
            }
            UpdateValue();
        }

        public void RemoveIncomingNode(int index)
        {
            if (index >= 0 && index < IncomingNodes.Length)
            {
                IncomingNodes[index] = null;
                UpdateValue();
            }
        }

        public void RemoveIncomingNode(IOutputNode outputNode)
        {
            for (int i = 0; i < IncomingNodes.Length; i++)
            {
                if (IncomingNodes[i] == outputNode)
                {
                    IncomingNodes[i] = null;
                }
            }

            UpdateValue();
        }

        public override void Close()
        {
            base.Close();

            RemoveAllIncomingNodes();
        }
    }
}