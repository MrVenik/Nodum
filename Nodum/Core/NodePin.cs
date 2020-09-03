using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Nodum.Core
{
    [Serializable]
    public abstract class NodePin
    {
        public bool IsInput { get; private set; }
        public bool IsOutput { get; private set; }
        public bool IsOption { get; private set; }
        public bool IsInternalInput { get; private set; }
        public bool IsInternalOutput { get; private set; }
        public bool IsInvokeUpdate { get; private set; }
        public bool IsInvokeUpdatePins { get; private set; }
        public bool CanSetValue { get; private set; }
        public bool CanGetValue { get; private set; }

        public Guid Guid { get; private set; }
        public string Name { get; set; }
        public Node Node { get; set; }

        private NodePinConnection _connection;

        public NodePin(string name, Node node, object[] attributes)
        {
            if (node != null)
            {
                Guid = Guid.NewGuid();
                Node = node;
                Name = name;

                if (attributes.FirstOrDefault(x => x is Node.NodePinAttribute) is Node.NodePinAttribute nodePinAttribute)
                {
                    IsInput = nodePinAttribute.IsInput;
                    IsOutput = nodePinAttribute.IsOutput;
                    IsOption = nodePinAttribute.IsOption;
                    IsInternalInput = nodePinAttribute.IsInternalInput;
                    IsInternalOutput = nodePinAttribute.IsInternalOutput;
                    IsInvokeUpdate = nodePinAttribute.IsInvokeUpdate;
                    IsInvokeUpdatePins = nodePinAttribute.IsInvokeUpdatePins;
                    CanSetValue = nodePinAttribute.CanSetValue;
                    CanGetValue = nodePinAttribute.CanGetValue;
                }
            }
            else throw new NodePinException("Node is null");
        }

        public NodePin(string name, Node node, NodePinOptions options)
        {
            if (node != null)
            {
                Guid = Guid.NewGuid();
                Name = name;
                Node = node;

                IsInput = options.IsInput;
                IsOutput = options.IsOutput;
                IsOption = options.IsOption;
                IsInternalInput = options.IsInternalInput;
                IsInternalOutput = options.IsInternalOutput;
                IsInvokeUpdate = options.IsInvokeUpdate;
                IsInvokeUpdatePins = options.IsInvokeUpdatePins;
                CanSetValue = options.CanSetValue;
                CanGetValue = options.CanGetValue;
            }
            else throw new NodePinException("Node is null");
        }

        protected object _value;
        public object Value
        {
            get => _value;
            set
            {
                _value = value;
                OnValueChanged?.Invoke();
            }
        }
        public virtual Type ValueType => Value?.GetType();


        [NonSerialized]
        private Action _onValueChanged;
        public Action OnValueChanged { get => _onValueChanged; set => _onValueChanged = value; }

        [NonSerialized]
        private Action _onNodePinChanged;
        public Action OnNodePinChanged { get => _onNodePinChanged; set => _onNodePinChanged = value; }


        [NonSerialized]
        private Func<NodePin, bool> _canConnectTo;
        protected Func<NodePin, bool> CanConnectTo { get => _canConnectTo; set => _canConnectTo = value; }
        public virtual void UpdateValue() { }

        public virtual void SetNodeValue(Node node) { }
        public virtual void GetNodeValue(Node node) { }

        public NodePin IncomingNodePin { get; private set; }
        public List<NodePin> OutgoingNodePins { get; private set; } = new List<NodePin>();

        public void AddOutgoingNodePin(NodePin inputNodePin)
        {
            if ((IsOutput || IsInternalOutput) && (inputNodePin.IsInput || inputNodePin.IsInternalInput))
            {
                OnValueChanged += inputNodePin.UpdateValue;
                OutgoingNodePins.Add(inputNodePin);

                OnNodePinChanged?.Invoke();
            }
        }

        public void RemoveOutgoingNodePin(NodePin inputNodePin)
        {
            OnValueChanged -= inputNodePin.UpdateValue;
            OutgoingNodePins.Remove(inputNodePin);

            OnNodePinChanged?.Invoke();
        }

        public void RemoveAllOutgoingNodePins()
        {
            OnValueChanged = null;

            for (int i = 0; i < OutgoingNodePins.Count; i++)
            {
                NodePin inputNodePin = OutgoingNodePins[i];

                inputNodePin.RemoveIncomingNodePin(this);

            }

            OutgoingNodePins.Clear();

            OnNodePinChanged?.Invoke();
        }

        public void AddIncomingNodePin(NodePin outputNodePin)
        {
            if (outputNodePin != null)
            {
                if (IsInput)
                {
                    if (outputNodePin.IsOutput)
                    {
                        if (Node != outputNodePin.Node && Node.Holder == outputNodePin.Node.Holder)
                        {
                            TryAddIncomingNodePin(outputNodePin);
                        }
                    }
                    else if (outputNodePin.IsInternalOutput)
                    {
                        if (Node.Holder == outputNodePin.Node)
                        {
                            TryAddIncomingNodePin(outputNodePin);
                        }
                    }
                }
                else if (IsInternalInput)
                {
                    if (outputNodePin.IsOutput)
                    {
                        if (Node == outputNodePin.Node.Holder)
                        {
                            TryAddIncomingNodePin(outputNodePin);
                        }
                    }
                    else if (outputNodePin.IsInternalOutput)
                    {
                        if (Node == outputNodePin.Node)
                        {
                            TryAddIncomingNodePin(outputNodePin);
                        }
                    }
                }
            }
        }

        private void CreateConnection(NodePin outputNodePin)
        {
            if (IsInput)
            {
                _connection = new NodePinConnection(outputNodePin, this);
                Node.IncomingConnections.Add(_connection);
            }
            else if (IsInternalInput)
            {
                _connection = new NodePinConnection(outputNodePin, this);
                Node.InternalIncomingConnections.Add(_connection);
            }
        }

        private bool TryAddIncomingNodePin(NodePin outputNodePin)
        {
            if (CanConnectTo == null || (CanConnectTo != null && CanConnectTo(outputNodePin)))
            {
                RemoveConnection();

                IncomingNodePin?.RemoveOutgoingNodePin(this);
                IncomingNodePin = outputNodePin;
                IncomingNodePin.AddOutgoingNodePin(this);

                CreateConnection(outputNodePin);

                OnNodePinChanged?.Invoke();

                UpdateValue();

                return true;
            }
            else return false;
        }

        public void RemoveIncomingNodePin(NodePin outputNodePin)
        {
            if (IncomingNodePin == outputNodePin)
            {
                RemoveConnection();

                IncomingNodePin?.RemoveOutgoingNodePin(this);
                IncomingNodePin = null;

                OnNodePinChanged?.Invoke();

                UpdateValue();
            }
        }

        private void RemoveConnection()
        {
            if (IsInternalInput)
            {
                Node.InternalIncomingConnections.Remove(_connection);
            }
            else
            {
                Node.IncomingConnections.Remove(_connection);
            }
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            if (Node != null)
            {
                Node.BindUpdates(this);
            }
            Guid = Guid.NewGuid();
        }

        public void ReConnect()
        {
            if (IncomingNodePin != null)
            {
                AddIncomingNodePin(IncomingNodePin);
            }
        }

        public void Close()
        {
            RemoveIncomingNodePin(IncomingNodePin);
            RemoveAllOutgoingNodePins();
        }
    }

    [Serializable]
    public class NodePin<T> : NodePin
    {
        public NodePin(string name, Node node, object[] attributes) : base(name, node, attributes)
        {
            Value = default;
        }

        public NodePin(string name, Node node, NodePinOptions options) : base(name, node, options)
        {
            Value = default;
        }

        public override Type ValueType => typeof(T);

        public new T Value
        {
            get => (T)_value;
            set
            {
                _value = value;
                OnValueChanged?.Invoke();
            }
        }
        public override void UpdateValue()
        {
            try
            {
                if (IncomingNodePin != null)
                {
                    Value = (T)IncomingNodePin.Value;
                }
                else
                {
                    Value = default;
                }
            }
            catch (Exception)
            {

            }
        }

        public override void SetNodeValue(Node node)
        {
        }

        public override void GetNodeValue(Node node)
        {
        }
    }
}
