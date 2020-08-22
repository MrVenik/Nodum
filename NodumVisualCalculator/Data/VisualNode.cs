using Nodum.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NodumVisualCalculator.Data
{
    public class MathNode : Node
    {
        public enum MathType
        {
            Add,
            Subtract,
            Multiply,
            Divide,
            Remainder

        }

        [NodePin(IsInvokeUpdate = true, CanSetValue = true)] public MathType Operation;
        [Input] public double InputA { get; set; }
        [Input] public double InputB { get; set; }
        [Output] public double Result { get; set; }

        public override void UpdateValue()
        {
            Result = 0;
            switch (Operation)
            {
                case MathType.Add:
                    Result = InputA + InputB;
                    break;
                case MathType.Subtract:
                    Result = InputA - InputB;
                    break;
                case MathType.Multiply:
                    Result = InputA * InputB;
                    break;
                case MathType.Divide:
                    if (InputB != 0)
                    {
                        Result = InputA / InputB;
                    }
                    break;
                case MathType.Remainder:
                    if (InputB != 0)
                    {
                        Result = InputA % InputB;
                    }
                    break;
            }
        }
    }

    public class NumberNode : Node
    {
        [InputOutput] public double Number;
    }

    public class EmptyNode : Node
    {

    }

    public class VisualNode
    {
        public List<VisualNodePin> VisualNodePins { get; private set; } = new List<VisualNodePin>();

        public List<VisualNode> InternalVisualNodes { get; private set; } = new List<VisualNode>();

        public VisualNode(VisualNode holder = null)
        {
            Holder = holder;
            Node = new EmptyNode();
            Editable = true;
        }

        public VisualNode(NodePin nodePin, VisualNode holder)
        {
            Holder = holder;
            Node = new EmptyNode();

            Editable = false;

            VisualNodePin visualNodePin = new VisualNodePin()
            {
                NodePin = nodePin,
                VisualNode = this,
                ElementId = $"{nodePin.Name}_{nodePin.Guid}",
                Showed = true,
                IsInternal = true
            };
            VisualNodePins.Add(visualNodePin);
        }

        public VisualNode(Node node, VisualNode holder)
        {
            Node = node;
            Holder = holder;

            Editable = false;

            foreach (var pin in node.AllNodePins)
            {
                VisualNodePin visualNodePin = new VisualNodePin()
                {
                    NodePin = pin,
                    VisualNode = this,
                    ElementId = $"{pin.Name}_{pin.Guid}",
                    Showed = true
                };
                VisualNodePins.Add(visualNodePin);
            }          
        }

        public List<NodePinConnection> IncomingConnections { get; private set; } = new List<NodePinConnection>();
        public List<NodePinConnection> OutgoingConnections { get; private set; } = new List<NodePinConnection>();

        public Node Node { get; private set; }
        public VisualNode Holder { get; private set; }
        public string Name
        {
            get => Node.Name;
            set
            {
                Node.Name = value;
            }
        }
        //public Guid Guid { get => Node.Guid; }
        public Position Position { get; set; } = new Position();
        public bool Editable { get; set; }
        public bool Showed { get; set; }
        public bool Focused { get; set; }
        public bool MenuShowed { get; set; }


        public void AddInputNodePin(string name, Type pinType)
        {
            bool isInput = true;
            bool isOutput = false;
            bool isInvokeUpdate = true;

            NodePin nodePin = NodeBuilder.BuildNodePin(name, pinType, isInput, isOutput, isInvokeUpdate);

            VisualNodePin visualNodePin = new VisualNodePin()
            {
                NodePin = nodePin,
                VisualNode = this,
                ElementId = $"{nodePin.Name}_{nodePin.Guid}",
                Showed = true
            };

            InternalVisualNodes.Add(new VisualNode(nodePin, this));
            VisualNodePins.Add(visualNodePin);
            Node.AddNodePin(nodePin);
        }

        public void AddOutputNodePin(string name, Type pinType)
        {
            bool isInput = false;
            bool isOutput = true;
            bool isInvokeUpdate = false;

            NodePin nodePin = NodeBuilder.BuildNodePin(name, pinType, isInput, isOutput, isInvokeUpdate);

            VisualNodePin visualNodePin = new VisualNodePin()
            {
                NodePin = nodePin,
                VisualNode = this,
                ElementId = $"{nodePin.Name}_{nodePin.Guid}",
                Showed = true
            };

            InternalVisualNodes.Add(new VisualNode(nodePin, this));

            VisualNodePins.Add(visualNodePin);
            Node.AddNodePin(nodePin);
        }

        public void AddNode(VisualNode visualNode)
        {
            InternalVisualNodes.Add(visualNode);
        }

        public void AddNodes(params VisualNode[] visualNodes)
        {
            foreach (var node in visualNodes)
            {
                AddNode(node);
            }
        }

        public void RemoveAllNodes()
        {
            foreach (var node in InternalVisualNodes)
            {
                node.Close();
            }
            InternalVisualNodes.Clear();
        }

        public void RemoveNode(VisualNode visualNode)
        {
            visualNode.Close();

            InternalVisualNodes.Remove(visualNode);
        }

        public void Close()
        {
            CloseIncomingConnections();
            CloseOutgoingConnections();

            RemoveAllNodes();

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
