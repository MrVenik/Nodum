using System;
using System.Collections.Generic;

namespace Nodum.Node
{
    public class ValueNode<T> : IValueNode<T>
    {
        public Position Position { get; set; } = new Position();
        public string Name { get; set; }
        public Guid Guid { get; private set; }
        public INodeHolder Holder { get; private set; }

        public NodePin OutputPin { get; private set; }

        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                OnValueChanged?.Invoke();
            }
        }
        object IValueNode.Value
        {
            get => _value;
            set
            {
                _value = (T)value;
                OnValueChanged?.Invoke();
            }
        }
        public Action OnValueChanged { get; set; }
        public Type ValueType { get => typeof(T); }


        public ValueNode(string name, INodeHolder node, bool showed = true, bool outputPinShowed = true, T value = default)
        {
            Guid = Guid.NewGuid();

            Name = name;
            Holder = node;
            Value = value;
            Showed = showed;

            OutputPin = new NodePin
            {
                Node = this,
                Position = new Position(),
                ElementId = $"{Name}_Output_{Guid}",
                Showed = outputPinShowed
            };


            if (Holder != null)
            {
                Holder.Position.OnPositionChanged += () => OutputPin.Position.UpdatePosition?.Invoke();
            }
        }

        public virtual void UpdateValue()
        {

        }

        protected virtual bool CanConnectTo(IOutputNode outputNode)
        {
            return true;
        }

        public bool Showed { get; set; }

        private List<IInputNode> _outgoingNodes = new List<IInputNode>();

        public virtual void AddOutgoingNode(IInputNode inputNode)
        {
            OnValueChanged += inputNode.UpdateValue;
            _outgoingNodes.Add(inputNode);
        }

        public virtual void RemoveOutgoingNode(IInputNode inputNode)
        {
            OnValueChanged -= inputNode.UpdateValue;
            _outgoingNodes.Remove(inputNode);
        }

        public virtual void RemoveAllOutgoingNodes()
        {
            OnValueChanged = null;

            for (int i = 0; i < _outgoingNodes.Count; i++)
            {
                IInputNode inputNode = _outgoingNodes[i];
                if (inputNode is ISingleInputNode singleInput)
                {
                    singleInput.RemoveIncomingNode();
                }
                else if (inputNode is IMultipleInputNode multiplyInput)
                {
                    multiplyInput.RemoveIncomingNode(this);
                }
                else if (inputNode is ICommutateInputNode commutateInputNode)
                {
                    commutateInputNode.RemoveIncomingNode(this);
                }
            }

            _outgoingNodes.Clear();
        }

        public virtual void Close()
        {
            RemoveAllOutgoingNodes();
        }
    }
}