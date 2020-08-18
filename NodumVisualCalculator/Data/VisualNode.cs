using Nodum.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NodumVisualCalculator.Data
{
    public class VisualNode : IVisualNode
    {
        public VisualNode(INode node, VisualNodeHolder holder = null)
        {
            Node = node;
            Holder = holder;

            if (Node is IOutputNode)
            {
                OutputNodePin = new NodePin()
                {
                    VisualNode = this,
                    Position = new Position(),
                    ElementId = $"{Name}_{Guid}_Output",
                    Showed = true
                };
                if (Holder != null)
                {
                    Holder.Position.OnPositionChanged += () => OutputNodePin.Position.UpdatePosition?.Invoke();
                }
            }
            if (Node is IInputNode inputNode)
            {
                InputNodePins = new List<NodePin>();
                for (int i = 0; i < inputNode.AmountOfInputs; i++)
                {
                    NodePin pin = new NodePin()
                    {
                        VisualNode = this,
                        Position = new Position(),
                        ElementId = $"{Name}_{Guid}_Input_{i}",
                        Showed = true
                    };
                    if (Holder != null)
                    {
                        Holder.Position.OnPositionChanged += () => pin.Position.UpdatePosition?.Invoke();
                    }
                    InputNodePins.Add(pin);
                }
            }
        }
        public NodePin OutputNodePin { get; private set; }
        public List<NodePin> InputNodePins { get; private set; }

        public List<NodePinConnection> IncomingConnections { get; private set; } = new List<NodePinConnection>();
        public List<NodePinConnection> OutgoingConnections { get; private set; } = new List<NodePinConnection>();

        public void ConnectToNode(VisualNode outputVisualNode, int inputIndex)
        {
            if (Node is ISingleInputNode singleInputNode)
            {
                singleInputNode.AddIncomingNode(outputVisualNode.Node as IOutputNode);
            }
            else if (Node is ICommutateInputNode commutateInputNode)
            {
                commutateInputNode.AddIncomingNode(outputVisualNode.Node as IOutputNode);
            }
            else if (Node is IMultipleInputNode multipleInputNode)
            {
                multipleInputNode.AddIncomingNode(outputVisualNode.Node as IOutputNode, inputIndex);
            }

            NodePinConnection connection = new NodePinConnection(outputVisualNode.OutputNodePin, GetInputNodePin(inputIndex));
            IncomingConnections.Add(connection);
            outputVisualNode.OutgoingConnections.Add(connection);
        }

        public NodePin GetInputNodePin(int index = 0)
        {
            if (InputNodePins.Count > 0)
            {
                if (index >= 0 && index < InputNodePins.Count)
                {
                    return InputNodePins[index];
                }
            }
            throw new ArgumentOutOfRangeException();
        }

        public INode Node { get; private set; }
        public VisualNodeHolder Holder { get; private set; }
        public string Name
        {
            get => Node.Name;
            set
            {
                Node.Name = value;
            }
        }
        public Guid Guid { get => Node.Guid; }
        public Position Position { get; set; } = new Position();
        public bool Showed { get; set; }
        public bool Focused { get; set; }
        public bool MenuShowed { get; set; }
        public void Close()
        {
            CloseIncomingConnections();
            CloseOutgoingConnections();
            Node.Close();
        }

        private void CloseIncomingConnections()
        {
            for (int i = 0; i < IncomingConnections.Count; i++)
            {
                NodePinConnection item = IncomingConnections[i];
                item.CloseConnection();
            }
        }

        private void CloseOutgoingConnections()
        {
            for (int i = 0; i < OutgoingConnections.Count; i++)
            {
                NodePinConnection item = OutgoingConnections[i];
                item.CloseConnection();
            }
        }
    }
}
