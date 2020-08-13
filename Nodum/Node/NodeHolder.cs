using System;
using System.Collections.Generic;

namespace Nodum.Node
{
    public class NodeHolder : INodeHolder
    {
        public string Name { get; set; }
        public List<INode> Nodes { get; set; } = new List<INode>();

        public Guid Guid { get; private set; }

        public Position Position { get; set; } = new Position();

        public bool Showed { get; set; }

        public INodeHolder Holder { get; }

        public NodeHolder(string name, INodeHolder holder = null, bool showed = true, params INode[] nodes)
        {
            Name = name;
            Showed = showed;
            Holder = holder;

            AddNodes(nodes);
        }

        public void AddNode(INode nodePin)
        {
            Nodes.Add(nodePin);
        }

        public void AddNodes(params INode[] nodePins)
        {
            foreach (var pin in nodePins)
            {
                AddNode(pin);
            }
        }

        public void RemoveAllNodes()
        {
            foreach (var nodePin in Nodes)
            {
                nodePin.Close();
            }
            Nodes.Clear();
        }

        public void RemoveNode(INode nodePin)
        {
            nodePin.Close();
            
            Nodes.Remove(nodePin);
        }

        public void Close()
        {
            RemoveAllNodes();
        }
    }
}