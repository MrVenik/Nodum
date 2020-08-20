﻿using Nodum.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NodumVisualCalculator.Data
{
    public class MathNode : Node
    {
        [Input] public int InputA;
        [Input] public int InputB;
        [Output] public int Result;

        public MathNode()
            : base()
        {
        }

        public override void UpdateValue()
        {
            Result = InputA + InputB;
        }
    }

    public class IntNode : Node
    {
        [InputOutput] public int Number;
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
                InputPosition = new Position(),
                OutputPosition = new Position(),
                ElementId = $"{nodePin.Name}_{nodePin.Guid}",
                Showed = true,
                IsInternal = true
            };
            if (Holder != null)
            {
                Holder.Position.OnPositionChanged += () => visualNodePin.InputPosition.UpdatePosition?.Invoke();
                Holder.Position.OnPositionChanged += () => visualNodePin.OutputPosition.UpdatePosition?.Invoke();
            }
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
                    InputPosition = new Position(),
                    OutputPosition = new Position(),
                    ElementId = $"{pin.Name}_{pin.Guid}",
                    Showed = true
                };
                if (Holder != null)
                {
                    Holder.Position.OnPositionChanged += () => visualNodePin.InputPosition.UpdatePosition?.Invoke();
                    Holder.Position.OnPositionChanged += () => visualNodePin.OutputPosition.UpdatePosition?.Invoke();
                }
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
            Type type = typeof(NodePin<>);
            Type genericType = type.MakeGenericType(pinType);

            bool isInput = true;
            bool isOutput = false;
            bool isInvokeUpdate = true;


            NodePin nodePin = (NodePin)Activator.CreateInstance(genericType, new object[] { name, isInput, isOutput, isInvokeUpdate, default });

            VisualNodePin visualNodePin = new VisualNodePin()
            {
                NodePin = nodePin,
                VisualNode = this,
                InputPosition = new Position(),
                OutputPosition = new Position(),
                ElementId = $"{nodePin.Name}_{nodePin.Guid}",
                Showed = true
            };
            if (Holder != null)
            {
                Holder.Position.OnPositionChanged += () => visualNodePin.InputPosition.UpdatePosition?.Invoke();
                Holder.Position.OnPositionChanged += () => visualNodePin.OutputPosition.UpdatePosition?.Invoke();
            }

            InternalVisualNodes.Add(new VisualNode(nodePin, this));
            VisualNodePins.Add(visualNodePin);
            Node.AddNodePin(nodePin);
        }

        public void AddOutputNodePin(string name, Type pinType)
        {
            Type type = typeof(NodePin<>);
            Type genericType = type.MakeGenericType(pinType);

            bool isInput = false;
            bool isOutput = true;
            bool isInvokeUpdate = false;


            NodePin nodePin = (NodePin)Activator.CreateInstance(genericType, new object[] { name, isInput, isOutput, isInvokeUpdate, default });

            VisualNodePin visualNodePin = new VisualNodePin()
            {
                NodePin = nodePin,
                VisualNode = this,
                InputPosition = new Position(),
                OutputPosition = new Position(),
                ElementId = $"{nodePin.Name}_{nodePin.Guid}",
                Showed = true
            };
            if (Holder != null)
            {
                Holder.Position.OnPositionChanged += () => visualNodePin.InputPosition.UpdatePosition?.Invoke();
                Holder.Position.OnPositionChanged += () => visualNodePin.OutputPosition.UpdatePosition?.Invoke();
            }

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
