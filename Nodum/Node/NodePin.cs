using System;
using System.Collections.Generic;
using System.Linq;

namespace Nodum.Node
{

    public abstract class NodePin
    {
        public bool IsInput { get; private set; }
        public bool IsOutput { get; private set; }
        public bool IsInvokeUpdate { get; private set; }

        public Guid Guid { get; private set; }
        public string Name { get; set; }

        protected NodePin(string name, object[] attributes)
        {
            Guid = Guid.NewGuid();

            Name = name;

            if (attributes.FirstOrDefault(x => x is Node.NodePinAttribute) is Node.NodePinAttribute nodePinAttribete)
            {
                IsInput = nodePinAttribete.IsInput;
                IsOutput = nodePinAttribete.IsOutput;
                IsInvokeUpdate = nodePinAttribete.IsInvokeUpdate;
            }
        }

        protected NodePin(string name, bool isInput = false, bool isOutput = false, bool isInvokeUpdate = false)
        {
            Guid = Guid.NewGuid();

            Name = name;
            IsInput = isInput;
            IsOutput = isOutput;
            IsInvokeUpdate = isInvokeUpdate;
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
        public Action OnValueChanged { get; set; }

        public abstract Type ValueType { get; }
        protected Func<NodePin, bool> CanConnectTo { get; set; }
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

        public void Close()
        {
            RemoveIncomingNodePin(IncomingNodePin);
            RemoveAllOutgoingNodePins();
        }
    }

    public class NodePin<T> : NodePin
    {
        public override Type ValueType { get => typeof(T); }

        public NodePin(string name, object[] attributes) : base(name, attributes)
        {
            Value = default;
        }

        public NodePin(string name, bool isInput = false, bool isOutput = false, bool isInvokeUpdate = false)
            : base(name, isInput, isOutput, isInvokeUpdate)
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
