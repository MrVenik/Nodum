using System;
using System.Collections.Generic;
using System.Linq;

namespace Nodum.Node
{
    public abstract class Node
    {
        public class NodePinAttribute : Attribute
        {
            public virtual bool IsInput { get; set; } = false;
            public virtual bool IsOutput { get; set; } = false;
            public virtual bool IsInvokeUpdate { get; set; } = false;
            public virtual bool CanSetValue { get; set; } = false;
            public virtual bool CanGetValue { get; set; } = false;
        }
        public class InputAttribute : NodePinAttribute
        {
            public override bool IsInput { get; set; } = true;
            public override bool IsOutput { get; set; } = false;
            public override bool IsInvokeUpdate { get; set; } = true;
            public override bool CanSetValue { get; set; } = true;
            public override bool CanGetValue { get; set; } = false;
        }

        public class OutputAttribute : NodePinAttribute
        {
            public override bool IsInput { get; set; } = false;
            public override bool IsOutput { get; set; } = true;
            public override bool IsInvokeUpdate { get; set; } = false;
            public override bool CanSetValue { get; set; } = false;
            public override bool CanGetValue { get; set; } = true;
        }

        public class InputOutputAttribute : NodePinAttribute
        {
            public override bool IsInput { get; set; } = true;
            public override bool IsOutput { get; set; } = true;
            public override bool IsInvokeUpdate { get; set; } = false;
            public override bool CanSetValue { get; set; } = true;
            public override bool CanGetValue { get; set; } = true;
        }

        public Dictionary<string, NodePin> NodePins { get; } = new Dictionary<string, NodePin>();
        public List<NodePin> AllInputNodePins => NodePins.Values.Where(p => p.IsInput).ToList();
        public List<NodePin> AllOutputNodePins => NodePins.Values.Where(p => p.IsOutput).ToList();
        public List<NodePin> AllNodePins => NodePins.Values.ToList();

        public string Name { get; set; }

        protected Node()
        {
            NodeBuilder.BuildNode(this);
            Update();
        }

        public bool TryAddNodePin(NodePin nodePin)
        {
            if (!NodePins.ContainsKey(nodePin.Name))
            {
                NodePins.Add(nodePin.Name, nodePin);
                if (nodePin.IsInvokeUpdate)
                {
                    nodePin.OnValueChanged += Update;
                }
                return true;
            }
            else return false;
        }

        public void RemoveNodePin(NodePin nodePin)
        {
            if (NodePins.ContainsKey(nodePin.Name))
            {
                NodePins[nodePin.Name].Close();
                NodePins.Remove(nodePin.Name);
            }
        }

        public void SetNodePin(NodePin nodePin)
        {
            if (NodePins.ContainsKey(nodePin.Name))
            {
                RemoveNodePin(nodePin);

                NodePins.Add(nodePin.Name, nodePin);

                if (nodePin.IsInvokeUpdate)
                {
                    nodePin.OnValueChanged += Update;
                }
            }
        }

        public virtual void UpdateValue()
        {

        }

        public void Update()
        {
            foreach (var pin in AllNodePins)
            {
                if (pin.CanSetValue)
                {
                    pin.SetNodeValue(this);
                }
            }

            UpdateValue();

            foreach (var pin in AllNodePins)
            {
                if (pin.CanGetValue)
                {
                    pin.GetNodeValue(this);
                }
            }
        }

        public void Close()
        {
            foreach (var pin in AllNodePins)
            {
                pin.Close();
            }
        }
    }
}
