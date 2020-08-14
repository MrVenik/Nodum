using System;
using System.Collections.Generic;

namespace Nodum.Node
{
    public class CommutateInputFuncNode<T> : CommutateInputValueNode<T>
    {
        private Func<List<IOutputNode>, T> _func;
        public CommutateInputFuncNode(string name, Func<List<IOutputNode>, T> func)
            : base(name)
        {
            _func = func;
        }

        public override void UpdateValue()
        {
            if (_func != null)
            {
                Value = _func.Invoke(IncomingNodes);
            }
            else
            {
                Value = default;
            }
        }
    }
}