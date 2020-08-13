using System;
using System.Collections.Generic;

namespace Nodum.Node
{
    public class CommutateInputFuncNode<T> : CommutateInputValueNode<T>
    {
        private Func<List<IOutputNode>, T> _func;
        public CommutateInputFuncNode(string name, INodeHolder node, Func<List<IOutputNode>, T> func, bool nodePinShowed = true)
            : base(name, node, nodePinShowed)
        {
            _func = func;
        }

        public override void UpdateValue()
        {
            if (_func != null)
            {
                Value = _func.Invoke(InputPin.IncomingNodes);
            }
            else
            {
                Value = default;
            }
        }
    }
}