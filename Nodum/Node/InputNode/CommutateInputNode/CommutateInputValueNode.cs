using System.Collections.Generic;

namespace Nodum.Node
{
    public class CommutateInputValueNode<T> : ValueNode<T>, ICommutateInputNode
    {
        public CommutateInputNodePin InputPin { get; private set; } = new CommutateInputNodePin();

        public CommutateInputValueNode(string name, INodeHolder node, bool nodeShowed = true, bool outputPinShowed = true, T value = default)
            : base(name, node, nodeShowed, outputPinShowed, value)
        {
            InputPin = new CommutateInputNodePin
            {
                Node = this,
                Position = new Position(),
                ElementId = $"{Name}_Input_{Guid}",
                Showed = true,
                IncomingNodes = new List<IOutputNode>(),
                IncomingConnections = new List<Connection>()

            };

            if (Holder != null)
            {
                Holder.Position.OnPositionChanged += () => InputPin.Position.UpdatePosition?.Invoke();
            }
        }

        public void AddIncomingNode(IOutputNode outputNode)
        {
            if (!InputPin.IncomingNodes.Contains(outputNode) && outputNode != this && CanConnectTo(outputNode))
            {
                outputNode.AddOutgoingNode(this);

                InputPin.IncomingNodes.Add(outputNode);
                InputPin.IncomingConnections.Add(new Connection(outputNode.OutputPin, InputPin));

                UpdateValue();
            }

        }

        public void AddIncomingNode(IOutputNode outputNode, int index)
        {
            if (index < InputPin.IncomingNodes.Count)
            {
                if (outputNode != this && CanConnectTo(outputNode))
                {
                    InputPin.IncomingNodes[index]?.RemoveOutgoingNode(this);
                    InputPin.IncomingNodes[index] = outputNode;
                    InputPin.IncomingNodes[index].AddOutgoingNode(this);
                    InputPin.IncomingConnections[index] = new Connection(outputNode.OutputPin, InputPin);
                    UpdateValue();
                }
            }
        }

        public Connection[] GetIncomingConnections()
        {
            return InputPin.IncomingConnections.ToArray();
        }

        public void RemoveAllIncomingNodes()
        {
            InputPin.IncomingNodes.Clear();
            InputPin.IncomingConnections.Clear();

            UpdateValue();
        }

        public void RemoveIncomingNode(IOutputNode outputNode)
        {
            InputPin.IncomingNodes.RemoveAll(p => p == outputNode);
            InputPin.IncomingConnections.RemoveAll(l => l.OutputNodePin == outputNode.OutputPin);

            UpdateValue();
        }

        public void RemoveIncomingNode(int index)
        {
            InputPin.IncomingNodes.RemoveAt(index);
            InputPin.IncomingConnections.RemoveAt(index);

            UpdateValue();
        }

        public override void Close()
        {
            base.Close();

            RemoveAllIncomingNodes();
        }
    }
}