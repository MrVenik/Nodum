using System;

namespace Nodum.Node
{
    public class SingleInputValueNode<T> : ValueNode<T>, ISingleInputNode
    {
        public int AmountOfInputs { get => 1; }
        public SingleInputValueNode(string name, T value = default)
            : base(name, value)
        {
        }

        public IOutputNode IncomingNode { get; private set; }

        public override void UpdateValue()
        {
            try
            {
                if (IncomingNode != null && IncomingNode is IValueNode valueNodePin)
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
                IncomingNode?.RemoveOutgoingNode(this);
                IncomingNode = outputNode;
                IncomingNode.AddOutgoingNode(this);
                UpdateValue();
            }
        }

        public void RemoveIncomingNode()
        {
            IncomingNode = null;
            UpdateValue();

        }

        public override void Close()
        {
            base.Close();

            RemoveIncomingNode();
        }
    }
}