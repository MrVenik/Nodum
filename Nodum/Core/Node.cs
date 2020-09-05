using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
            public virtual bool IsOption { get; set; } = false;
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

        public double PositionX { get; set; }
        public double PositionY { get; set; }

        public Node Holder { get; set; }
        public virtual bool IsEditable { get; }
        public bool IsInternal => Holder != null;

        [NonSerialized]
        private Action _onUpdatePins;
        public Action OnUpdatePins { get => _onUpdatePins; set => _onUpdatePins = value; }

        [NonSerialized]
        private Action _onNodeChanged;
        public Action OnNodeChanged { get => _onNodeChanged; set => _onNodeChanged = value; }

        public Dictionary<string, NodePin> NodePins { get; protected set; } = new Dictionary<string, NodePin>();
        public IEnumerable<NodePin> AllInputNodePins => NodePins.Values.Where(p => p.IsInput);
        public IEnumerable<NodePin> AllOutputNodePins => NodePins.Values.Where(p => p.IsOutput);
        public IEnumerable<NodePin> AllOptionNodePins => NodePins.Values.Where(p => p.IsOption);
        public IEnumerable<NodePin> AllNodePins => NodePins.Values.ToList();
        public List<Node> InternalNodes { get; protected set; } = new List<Node>();

        public List<NodePinConnection> IncomingConnections { get; protected set; } = new List<NodePinConnection>();
        public List<NodePinConnection> InternalIncomingConnections { get; protected set; } = new List<NodePinConnection>();
        public List<NodePinConnection> OutgoingConnections { get; protected set; } = new List<NodePinConnection>();

        static Node()
        {
            NodeCacher.CacheBaseNodes();
        }

        protected Node(string name)
        {
            Name = name;

            BuildNode();

            Update();
        }

        private void BuildNode()
        {
            Type nodeType = GetType();
            NodeMembersInfo members = NodeCacher.GetNodeMembers(nodeType);
            if (members != null)
            {
                BuildFieldNodePins(members.FieldInfoList);
                BuildPropertyNodePins(members.PropertyInfoList);
            }
        }

        private void BuildFieldNodePins(List<FieldInfo> fields)
        {
            foreach (var field in fields)
            {
                NodePin nodePin = NodePinBuilder.BuildNodePin(field, this);
                ProtectedTryAddNodePin(nodePin);
            }
        }

        private void BuildPropertyNodePins(List<PropertyInfo> properties)
        {
            foreach (var property in properties)
            {
                NodePin nodePin = NodePinBuilder.BuildNodePin(property, this);
                ProtectedTryAddNodePin(nodePin);
            }
        }

        public void AddInternalNode(Node node)
        {
            if (IsEditable)
            {
                node.Holder = this;
                InternalNodes.Add(node);
                OnNodeChanged?.Invoke();
            }
            else throw new Exception($"Can't modify Node. Node.IsEditable = {IsEditable}");
        }

        public void RemoveInternalNode(Node node)
        {
            if (IsEditable)
            {
                InternalNodes.Remove(node);
                OnNodeChanged?.Invoke();
            }
            else throw new Exception($"Can't modify Node. Node.IsEditable = {IsEditable}");
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

            nodePin.OnNodePinChanged += () => OnNodeChanged?.Invoke();
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
                ProtectedRemoveNodePin(nodePin);

                NodePins.Add(nodePin.Name, nodePin);

                if (nodePin.IsInvokeUpdate)
                {
                    nodePin.OnValueChanged += Update;
                }
            }
        }

        public bool TryAddNodePin(NodePin nodePin)
        {
            if (IsEditable)
            {
                bool succes = ProtectedTryAddNodePin(nodePin);
                OnNodeChanged?.Invoke();
                return succes;
            }
            else throw new Exception($"Can't modify Node. Node.IsEditable = {IsEditable}");
        }

        public void RemoveNodePin(NodePin nodePin)
        {
            if (IsEditable)
            {
                ProtectedRemoveNodePin(nodePin);
                OnNodeChanged?.Invoke();
            }
            else throw new Exception($"Can't modify Node. Node.IsEditable = {IsEditable}");
        }

        public void RemoveNodePin(string nodePinName)
        {
            if (IsEditable)
            {
                ProtectedRemoveNodePin(nodePinName);
                OnNodeChanged?.Invoke();
            }
            else throw new Exception($"Can't modify Node. Node.IsEditable = {IsEditable}");
        }

        public void SetNodePin(NodePin nodePin)
        {
            if (IsEditable)
            {
                ProtectedSetNodePin(nodePin);
                OnNodeChanged?.Invoke();
            }
            else throw new Exception($"Can't modify Node. Node.IsEditable = {IsEditable}");
        }

        [OnDeserialized]
        public virtual void OnDeserialized(StreamingContext context)
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

        public static Node CloneNode(Node node)
        {
            if (node != null)
            {
                BinaryNodeSerializer serializer = new BinaryNodeSerializer();
                byte[] bytes = serializer.SerializeToByteArray(node);
                return serializer.DeserializeFromByteArray(bytes);
            }
            throw new Exception("Node is null");
        }

        public virtual Node Clone()
        {
            return CloneNode(this);
        }

        public virtual string GetStringForNodePin(NodePin nodePin)
        {
            if (nodePin.Node == this)
            {
                return nodePin.IncomingNodePin == null ? 
                    ((nodePin.Node.Holder != null && nodePin.Node.Holder != this ) ? 
                    (nodePin.ValueType == typeof(bool) ? 
                    nodePin.Value.ToString().ToLower() : 
                    nodePin.Value.ToString()) : nodePin.Name) : 
                    nodePin.IncomingNodePin.Node.GetStringForNodePin(nodePin.IncomingNodePin);
            }
            else throw new NodeException($"Can't get string for nodePin {nodePin.Name}. NodePin is not in this Node");
        }

        [NonSerialized]
        protected Dictionary<string, ParameterExpression> Parameters;

        public virtual Expression GetExpressionForNodePin(NodePin nodePin)
        {
            if (nodePin.Node == this)
            {
                if (nodePin.IncomingNodePin == null)
                {
                    if (nodePin.Node.Holder != null && nodePin.Node.Holder != this)
                    {
                        return Expression.Constant(nodePin.Value, nodePin.ValueType);
                    }
                    else
                    {
                        Parameters ??= new Dictionary<string, ParameterExpression>();

                        if (!Parameters.ContainsKey(nodePin.Name))
                        {
                            Parameters.Add(nodePin.Name, Expression.Parameter(nodePin.ValueType, nodePin.Name));
                        }

                        return Parameters[nodePin.Name];
                    }
                }
                else
                {
                    return nodePin.IncomingNodePin.Node.GetExpressionForNodePin(nodePin.IncomingNodePin);
                }
            }
            else throw new NodeException($"Can't get Expression for nodePin {nodePin.Name}. NodePin is not in this Node");
        }

        public virtual void Close()
        {
            foreach (var pin in AllNodePins)
            {
                pin.Close();
            }
        }
    }
}
