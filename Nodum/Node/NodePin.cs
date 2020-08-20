using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nodum.Node
{
    public abstract class NodePin
    {
        public bool IsInput { get; private set; }
        public bool IsOutput { get; private set; }
        public bool IsInvokeUpdate { get; private set; }

        public Guid Guid { get; private set; }
        public string Name { get; set; }

        public FieldInfo FieldInfo { get; private set; }

        protected NodePin(FieldInfo fieldInfo)
        {
            Guid = Guid.NewGuid();

            FieldInfo = fieldInfo;

            Name = FieldInfo.Name;

            object[] attribs = FieldInfo.GetCustomAttributes(true);
            if (attribs.FirstOrDefault(x => x is Node.NodePinAttribute) is Node.NodePinAttribute nodePinAttribete)
            {
                IsInput = nodePinAttribete.IsInput;
                IsOutput = nodePinAttribete.IsOutput;
                IsInvokeUpdate = nodePinAttribete.IsInvokeUpdate;
            }
        }

        protected NodePin(string name, bool isInput = false, bool isOutput = false, bool isInvokeUpdate = false)
        {
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

        public NodePin(FieldInfo fieldInfo) : base(fieldInfo)
        {
            Value = default;
        }

        public NodePin(FieldInfo fieldInfo, T value) : base(fieldInfo)
        {
            Value = value;
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
    }
}
