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
            Node.Close();
        }
    }
}
