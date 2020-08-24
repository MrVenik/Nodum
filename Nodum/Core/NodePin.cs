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
        public bool IsInternalInput { get; private set; }
        public bool IsInternalOutput { get; private set; }
        public bool IsInvokeUpdate { get; private set; }
        public bool IsInvokeUpdatePins { get; private set; }
        public bool CanSetValue { get; private set; }
        public bool CanGetValue { get; private set; }

        public Guid Guid { get; private set; }
        public string Name { get; set; }
        public Node Node { get; private set; }

        protected NodePin(string name, Node node, object[] attributes)
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
                    IsInternalInput = nodePinAttribute.IsInternalInput;
                    IsInternalOutput = nodePinAttribute.IsInternalOutput;
                    IsInvokeUpdate = nodePinAttribute.IsInvokeUpdate;
                    IsInvokeUpdatePins = nodePinAttribute.IsInvokeUpdatePins;
                    CanSetValue = nodePinAttribute.CanSetValue;
                    CanGetValue = nodePinAttribute.CanGetValue;
                }
            }
            else throw new Exception("Node is null");
        }

        protected NodePin(string name, Node node, bool isInput, bool isOutput, bool isInternalInput, bool isInternalOutput, bool isInvokeUpdate, bool isInvokeUpdatePins, bool canSetValue, bool canGetValue)
        {
            if (node != null)
            {
                Guid = Guid.NewGuid();
                Name = name;
                Node = node;

                IsInput = isInput;
                IsOutput = isOutput;
                IsInternalInput = isInternalInput;
                IsInternalOutput = isInternalOutput;
                IsInvokeUpdate = isInvokeUpdate;
                IsInvokeUpdatePins = isInvokeUpdatePins;
                CanSetValue = canSetValue;
                CanGetValue = canGetValue;
            }
            else throw new Exception("Node is null");
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
        public abstract Type ValueType { get; }


        [NonSerialized]
        private Action _onValueChanged;
        public Action OnValueChanged { get => _onValueChanged; set => _onValueChanged = value; }


        [NonSerialized]
        private Func<NodePin, bool> _canConnectTo;
        protected Func<NodePin, bool> CanConnectTo { get => _canConnectTo; set => _canConnectTo = value; }
        public abstract void UpdateValue();

        public abstract void SetNodeValue(Node node);
        public abstract void GetNodeValue(Node node);

        public NodePin IncomingNodePin { get; private set; }
        private List<NodePin> _outgoingNodePins = new List<NodePin>();

        public void AddOutgoingNodePin(NodePin inputNodePin)
        {
            OnValueChanged += inputNodePin.UpdateValue;
            _outgoingNodePins.Add(inputNodePin);
        }

        public void RemoveOutgoingNodePin(NodePin inputNodePin)
        {
            OnValueChanged -= inputNodePin.UpdateValue;
            _outgoingNodePins.Remove(inputNodePin);
        }

        public void RemoveAllOutgoingNodePins()
        {
            OnValueChanged = null;

            for (int i = 0; i < _outgoingNodePins.Count; i++)
            {
                NodePin inputNodePin = _outgoingNodePins[i];

                inputNodePin.RemoveIncomingNodePin(this);

            }

            _outgoingNodePins.Clear();
        }

        public void AddIncomingNodePin(NodePin outputNodePin)
        {
            if (CanConnectTo == null || (CanConnectTo != null && CanConnectTo(outputNodePin)))
            {
                IncomingNodePin?.RemoveOutgoingNodePin(this);
                IncomingNodePin = outputNodePin;
                IncomingNodePin.AddOutgoingNodePin(this);
                UpdateValue();
            }
        }

        public void RemoveIncomingNodePin(NodePin outputNodePin)
        {
            if (IncomingNodePin == outputNodePin)
            {
                IncomingNodePin = null;
                UpdateValue();
            }
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            if (Node != null)
            {
                Node.BindUpdates(this);
            }
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
        public override Type ValueType { get => typeof(T); }

        public NodePin(string name, Node node, object[] attributes) : base(name, node, attributes)
        {
            Value = default;
        }

        protected NodePin(string name, Node node, bool isInput, bool isOutput, bool isInternalInput, bool isInternalOutput, bool isInvokeUpdate, bool isInvokeUpdatePins, bool canSetValue, bool canGetValue)
            : base(name, node, isInput, isOutput, isInternalInput, isInternalOutput, isInvokeUpdate, isInvokeUpdatePins, canSetValue, canGetValue)
        {
            Value = default;
        }

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
