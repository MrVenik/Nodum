using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Nodum.Core
{
    [Serializable]
    public abstract class Node
    {
        public class NodePinAttribute : Attribute
        {
            public virtual bool IsInput { get; set; } = false;
            public virtual bool IsOutput { get; set; } = false;
            public virtual bool IsInternalInput { get; set; } = false;
            public virtual bool IsInternalOutput { get; set; } = false;
            public virtual bool IsInvokeUpdate { get; set; } = false;
            public virtual bool IsInvokeUpdatePins { get; set; } = false;
            public virtual bool CanSetValue { get; set; } = false;
            public virtual bool CanGetValue { get; set; } = false;
        }
        public class InputAttribute : NodePinAttribute
        {
            public override bool IsInput { get; set; } = true;
            public override bool IsInvokeUpdate { get; set; } = true;
            public override bool CanSetValue { get; set; } = true;
        }
        public class OutputAttribute : NodePinAttribute
        {
            public override bool IsOutput { get; set; } = true;
            public override bool CanGetValue { get; set; } = true;
        }
        public class InputOutputAttribute : NodePinAttribute
        {
            public override bool IsInput { get; set; } = true;
            public override bool IsOutput { get; set; } = true;
            public override bool CanSetValue { get; set; } = true;
            public override bool CanGetValue { get; set; } = true;
        }

        public string Name { get; set; }

        public Node Holder { get; private set; }
        public abstract bool IsBaseNode { get; }

        [NonSerialized]
        private Action _onUpdatePins;
        public Action OnUpdatePins { get => _onUpdatePins; set => _onUpdatePins = value; }

        public Dictionary<string, NodePin> NodePins { get; } = new Dictionary<string, NodePin>();
        public IEnumerable<NodePin> AllInputNodePins => NodePins.Values.Where(p => p.IsInput);
        public IEnumerable<NodePin> AllOutputNodePins => NodePins.Values.Where(p => p.IsOutput);
        public IEnumerable<NodePin> AllNodePins => NodePins.Values.ToList();
        public List<Node> InternalNodes { get; private set; } = new List<Node>();

        protected Node(string name, Node holder = null)
        {
            Name = name;
            Holder = holder;
            NodeBuilder.BuildNode(this);
            Update();
        }

        public void AddInternalNode(Node node)
        {
            if (!IsBaseNode)
            {
                InternalNodes.Add(node);
            }
            else throw new Exception($"Can't modify BaseNode. Node.IsBaseNode = {IsBaseNode}");
        }

        public void RemoveInternalNode(Node node)
        {
            if (!IsBaseNode)
            {
                InternalNodes.Remove(node);
            }
            else throw new Exception($"Can't modify BaseNode. Node.IsBaseNode = {IsBaseNode}");
        }

        public NodePin FindInputNodePin(Guid guid)
        {
            NodePin nodePin = null;
            if (Holder != null)
            {
                foreach (var node in Holder.InternalNodes)
                {
                    nodePin = node.AllInputNodePins.FirstOrDefault(pin => pin.Guid == guid);
                    break;
                }
            }
            return nodePin;
        }

        public NodePin FindOutputNodePin(Guid guid)
        {
            NodePin nodePin = null;
            if (Holder != null)
            {
                foreach (var node in Holder.InternalNodes)
                {
                    nodePin = node.AllOutputNodePins.FirstOrDefault(pin => pin.Guid == guid);
                    break;
                }
            }
            return nodePin;
        }

        public NodePin FindInternalInputNodePin(Guid guid)
        {
            throw new NotImplementedException();
        }

        public NodePin FindInternalOutputNodePin(Guid guid)
        {
            throw new NotImplementedException();
        }

        public void BindUpdates(NodePin nodePin)
        {
            if (nodePin.IsInvokeUpdate)
            {
                nodePin.OnValueChanged += Update;
            }
            if (nodePin.IsInvokeUpdatePins)
            {
                nodePin.OnValueChanged += UpdateAllPins;
            }
        }

        public void ReConnectAllPins()
        {
            foreach (var pin in AllNodePins)
            {
                pin.ReConnect();
            }
            if (InternalNodes.Count > 0)
            {
                foreach (var node in InternalNodes)
                {
                    node.ReConnectAllPins();
                }
            }
        }

        protected bool ProtectedTryAddNodePin(NodePin nodePin)
        {
            if (!NodePins.ContainsKey(nodePin.Name))
            {
                NodePins.Add(nodePin.Name, nodePin);
                BindUpdates(nodePin);
                return true;
            }
            else return false;
        }

        protected void ProtectedRemoveNodePin(NodePin nodePin)
        {
            if (NodePins.ContainsKey(nodePin.Name))
            {
                NodePins[nodePin.Name].Close();
                NodePins.Remove(nodePin.Name);
            }
        }

        protected void ProtectedRemoveNodePin(string nodePinName)
        {
            if (NodePins.ContainsKey(nodePinName))
            {
                NodePins[nodePinName].Close();
                NodePins.Remove(nodePinName);
            }
        }

        protected void ProtectedSetNodePin(NodePin nodePin)
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

        public bool TryAddNodePin(NodePin nodePin)
        {
            if (!IsBaseNode)
            {
                return ProtectedTryAddNodePin(nodePin);
            }
            else throw new Exception($"Can't modify BaseNode. Node.IsBaseNode = {IsBaseNode}");
        }

        public void RemoveNodePin(NodePin nodePin)
        {
            if (!IsBaseNode)
            {
                ProtectedRemoveNodePin(nodePin);
            }
            else throw new Exception($"Can't modify BaseNode. Node.IsBaseNode = {IsBaseNode}");
        }

        public void RemoveNodePin(string nodePinName)
        {
            if (!IsBaseNode)
            {
                ProtectedRemoveNodePin(nodePinName);
            }
            else throw new Exception($"Can't modify BaseNode. Node.IsBaseNode = {IsBaseNode}");
        }

        public void SetNodePin(NodePin nodePin)
        {
            if (!IsBaseNode)
            {
                ProtectedSetNodePin(nodePin);
            }
            else throw new Exception($"Can't modify BaseNode. Node.IsBaseNode = {IsBaseNode}");
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {

        }

        public virtual void UpdatePins()
        {

        }

        public void UpdateAllPins()
        {
            foreach (var pin in AllNodePins)
            {
                if (pin.CanSetValue)
                {
                    pin.SetNodeValue(this);
                }
            }

            UpdatePins();
            UpdateValue();

            foreach (var pin in AllNodePins)
            {
                if (pin.CanGetValue)
                {
                    pin.GetNodeValue(this);
                }
            }

            OnUpdatePins?.Invoke();
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
