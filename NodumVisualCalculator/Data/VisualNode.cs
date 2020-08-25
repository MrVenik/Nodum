using Nodum.Core;
using Nodum.Calc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NodumVisualCalculator.Data
{
    public class VisualNode
    {
        public List<VisualNodePin> VisualNodePins { get; private set; } = new List<VisualNodePin>();

        public List<VisualNode> InternalVisualNodes { get; private set; } = new List<VisualNode>();

        public VisualNode(string name, VisualNode holder = null)
        {
            Holder = holder;
            Node = new EmptyNode() { Name = name };
        }

        public VisualNode(NodePin nodePin, VisualNode holder)
        {
            Holder = holder;
            Node = new EmptyNode() { Name = nodePin.Name };

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

            Node.OnUpdatePins += () => ReAddNodePins(false);

            ReAddNodePins(true);

            if (node.InternalNodes.Count > 0)
            {
                foreach (var internalNode in node.InternalNodes)
                {
                    InternalVisualNodes.Add(new VisualNode(internalNode, this));
                }
            }
        }

        private void ReAddNodePins(bool firstLoad)
        {
            foreach (var pin in Node.AllNodePins)
            {
                if (!VisualNodePins.Any(vn => vn.NodePin == pin))
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
            if (!firstLoad)
            {
                List<VisualNodePin> toRemove = new List<VisualNodePin>();
                foreach (var visualPin in VisualNodePins)
                {
                    if (!Node.AllNodePins.Contains(visualPin.NodePin))
                    {
                        toRemove.Add(visualPin);
                    }
                }
                foreach (var visualPin in toRemove)
                {
                    VisualNodePins.Remove(visualPin);
                }
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
        public bool Showed { get; set; }
        public bool Focused { get; set; }
        public bool MenuShowed { get; set; }

        public void AddVisualNodePin(NodePin nodePin)
        {
            if (Node.TryAddNodePin(nodePin))
            {
                VisualNodePin visualNodePin = new VisualNodePin()
                {
                    NodePin = nodePin,
                    VisualNode = this,
                    ElementId = $"{nodePin.Name}_{nodePin.Guid}",
                    Showed = true
                };

                InternalVisualNodes.Add(new VisualNode(nodePin, this));

                VisualNodePins.Add(visualNodePin);
            }
        }

        public void AddNode(VisualNode visualNode)
        {
            InternalVisualNodes.Add(visualNode);
            Node.AddInternalNode(visualNode.Node);
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
                Node.RemoveInternalNode(node.Node);
            }
            InternalVisualNodes.Clear();
        }

        public void RemoveNode(VisualNode visualNode)
        {
            visualNode.Close();

            InternalVisualNodes.Remove(visualNode);
            Node.RemoveInternalNode(visualNode.Node);
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
