using System;

namespace Nodum.Node
{
    public class SingleInputFuncNode<T> : SingleInputValueNode<T>
    {
        private Func<IValueNode, T> _func;
        public SingleInputFuncNode(string name, INodeHolder node, Func<IValueNode, T> func, bool nodeShowed = true)
            : base(name, node, nodeShowed)
        {
            _func = func;
        }

        public override void UpdateValue()
        {
            if (_func != null && InputPin.IncomingNode != null && InputPin.IncomingNode is IValueNode valueNode)
            {
                Value = _func.Invoke(valueNode);
            }
        }
    }
}