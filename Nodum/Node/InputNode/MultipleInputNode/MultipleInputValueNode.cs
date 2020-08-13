using System.Collections.Generic;
using System.Linq;

namespace Nodum.Node
{
    public class MultipleInputValueNode<T> : ValueNode<T>, IMultipleInputNode
    {
        public InputNodePin[] InputPins { get; private set; }

        public MultipleInputValueNode(string name, INodeHolder node, int amountOfInputs, bool nodeShowed = true, bool outputNodePinShowed = true, T value = default)
            : base(name, node, nodeShowed, outputNodePinShowed, value)
        {
            InputPins = new InputNodePin[amountOfInputs];
            for (int i = 0; i < InputPins.Length; i++)
            {
                InputPins[i] = new InputNodePin
                {
                    Node = this,
                    Position = new Position(),
                    ElementId = $"{Name}_Input_{i}_{Guid}",
                    Showed = true
                };
            }

            foreach (var pin in InputPins)
            {
                if (Holder != null)
                {
                    Holder.Position.OnPositionChanged += () => pin.Position.UpdatePosition?.Invoke();
                }
            }
        }

        public void AddIncomingNode(IOutputNode outputNode, int index)
        {
            if (index < InputPins.Length && outputNode != this && CanConnectTo(outputNode))
            {
                InputPins[index].IncomingNode?.RemoveOutgoingNode(this);
                InputPins[index].IncomingNode = outputNode;
                InputPins[index].IncomingNode.AddOutgoingNode(this);
                InputPins[index].IncomingConnection = new Connection(InputPins[index].IncomingNode.OutputPin, InputPins[index]);
                UpdateValue();
            }
        }

        public void RemoveAllIncomingNodes()
        {
            for (int i = 0; i < InputPins.Length; i++)
            {
                RemoveIncomingNode(i);
            }
            UpdateValue();
        }

        public void RemoveIncomingNode(int index)
        {
            if (index >= 0 && index < InputPins.Length)
            {
                InputPins[index].IncomingNode = null;
                InputPins[index].IncomingConnection = null;
                UpdateValue();
            }
        }

        public void RemoveIncomingNode(IOutputNode outputNode)
        {
            foreach (var pin in InputPins.Where(p => p.IncomingNode == outputNode))
            {
                if (pin != null)
                {
                    pin.IncomingNode = null;
                    pin.IncomingConnection = null;
                }
            }

            UpdateValue();
        }

        public override void Close()
        {
            base.Close();

            RemoveAllIncomingNodes();
        }

        public Connection[] GetIncomingConnections()
        {
            List<Connection> lines = new List<Connection>();
            for (int i = 0; i < InputPins.Length; i++)
            {
                lines.Add(InputPins[i].IncomingConnection);
            }
            return lines.ToArray();
        }
    }
}