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
        }
        public class InputAttribute : NodePinAttribute
        {
            public override bool IsInput { get; set; } = true;
            public override bool IsOutput { get; set; } = false;
            public override bool IsInvokeUpdate { get; set; } = true;
        }

        public class OutputAttribute : NodePinAttribute
        {
            public override bool IsInput { get; set; } = false;
            public override bool IsOutput { get; set; } = true;
            public override bool IsInvokeUpdate { get; set; } = false;
        }

        public class InputOutputAttribute : NodePinAttribute
        {
            public override bool IsInput { get; set; } = true;
            public override bool IsOutput { get; set; } = true;
            public override bool IsInvokeUpdate { get; set; } = false;
        }

        public Dictionary<string, NodePin> NodePins { get; } = new Dictionary<string, NodePin>();
        public List<NodePin> AllNodePins => NodePins.Values.ToList();

        public string Name { get; set; }

        protected Node()
        {
            NodeBuilder.BuildNode(this);
            Update();
        }

        public void AddNodePin(NodePin nodePin)
        {
            if (!NodePins.ContainsKey(nodePin.Name))
            {
                NodePins.Add(nodePin.Name, nodePin);
                if (nodePin.IsInvokeUpdate)
                {
                    nodePin.OnValueChanged += Update;
                }
            }
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
                if (pin.IsInput && pin.FieldInfo != null)
                {
                    pin.FieldInfo.SetValue(this, pin.Value);
                }
            }

            UpdateValue();

            foreach (var pin in AllNodePins)
            {
                if (pin.IsOutput && pin.FieldInfo != null)
                {
                    pin.Value = pin.FieldInfo.GetValue(this);
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
