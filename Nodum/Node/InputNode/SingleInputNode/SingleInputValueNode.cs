using System;

namespace Nodum.Node
{
    public class SingleInputValueNode<T> : ValueNode<T>, ISingleInputNode
    {
        public SingleInputValueNode(string name, INodeHolder node, bool nodeShowed = true, bool outputNodePinShowed = true, T value = default)
            : base(name, node, nodeShowed, outputNodePinShowed, value)
        {
            InputPin = new InputNodePin
            {
                Node = this,
                Position = new Position(),
                ElementId = $"{Name}_Input_{Guid}",
                Showed = true

            };

            if (Holder != null)
            {
                Holder.Position.OnPositionChanged += () => InputPin.Position.UpdatePosition?.Invoke();
            }
        }

        public InputNodePin InputPin { get; private set; }

        public override void UpdateValue()
        {
            try
            {
                if (InputPin.IncomingNode != null && InputPin.IncomingNode is IValueNode valueNodePin)
                {
                    Value = (T)valueNodePin.Value;
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

        public void AddIncomingNode(IOutputNode outputNode)
        {
            if (outputNode != this && CanConnectTo(outputNode))
            {
                InputPin.IncomingNode?.RemoveOutgoingNode(this);
                InputPin.IncomingNode = outputNode;
                InputPin.IncomingNode.AddOutgoingNode(this);
                InputPin.IncomingConnection = new Connection(InputPin.IncomingNode.OutputPin, InputPin);
                UpdateValue();
            }
        }

        public void RemoveIncomingNode()
        {
            InputPin.IncomingNode = null;
            InputPin.IncomingConnection = null;
            UpdateValue();

        }

        public override void Close()
        {
            base.Close();

            RemoveIncomingNode();
        }

        public Connection[] GetIncomingConnections()
        {
            return new Connection[] { InputPin.IncomingConnection };
        }
    }
}