using System;

namespace Nodum.Node
{
    public class SingleInputFuncNode<T> : SingleInputValueNode<T>
    {
        private Func<IValueNode, T> _func;
        public SingleInputFuncNode(string name, Func<IValueNode, T> func)
            : base(name)
        {
            _func = func;
        }

        public override void UpdateValue()
        {
            if (_func != null && IncomingNode != null && IncomingNode is IValueNode valueNode)
            {
                Value = _func.Invoke(valueNode);
            }
        }
    }
}