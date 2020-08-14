using System.Collections.Generic;

namespace NodumVisualCalculator.Data
{
    public class VisualNodeHolder : IVisualNode
    {
        public VisualNodeHolder(string name, VisualNodeHolder holder = null)
        {
            Name = name;
            Holder = holder;
        }

        public List<IVisualNode> VisualNodes { get; private set; } = new List<IVisualNode>();
        public VisualNodeHolder Holder { get; private set; }
        public string Name { get; set; }
        public Position Position { get; set; } = new Position();
        public bool Showed { get; set; }
        public bool Focused { get; set; }
        public bool MenuShowed { get; set; }

        public void AddNode(IVisualNode visualNode)
        {
            VisualNodes.Add(visualNode);
        }

        public void AddNodes(params IVisualNode[] visualNodes)
        {
            foreach (var node in visualNodes)
            {
                AddNode(node);
            }
        }

        public void RemoveAllNodes()
        {
            foreach (var node in VisualNodes)
            {
                node.Close();
            }
            VisualNodes.Clear();
        }

        public void RemoveNode(IVisualNode visualNode)
        {
            visualNode.Close();

            VisualNodes.Remove(visualNode);
        }

        public void Close()
        {
            RemoveAllNodes();
        }
    }
}
