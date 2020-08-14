using System;
using System.Collections.Generic;

namespace Nodum.Node
{
    public class MultipleInputFuncNode<T> : MultipleInputValueNode<T>
    {
        private Func<IValueNode[], T> _func;
        public MultipleInputFuncNode(string name, int amountOfInputs, Func<IValueNode[], T> func)
            : base(name, amountOfInputs)
        {
            _func = func;
        }

        public override void UpdateValue()
        {
            if (_func != null)
            {
                List<IValueNode> inputs = new List<IValueNode>();

                for (int i = 0; i < IncomingNodes.Length; i++)
                {
                    if (IncomingNodes[i] != null && IncomingNodes[i] is IValueNode valueNode)
                    {
                        inputs.Add(valueNode);
                    }
                    else break;
                }
                Value = _func.Invoke(inputs.ToArray());

            }
            else
            {
                Value = default;
            }
        }
    }
}