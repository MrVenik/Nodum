using System;
using System.Collections.Generic;

namespace Nodum.Node
{
    public class MultipleInputFuncNode<T> : MultipleInputValueNode<T>
    {
        private Func<IValueNode[], T> _func;
        public MultipleInputFuncNode(string name, INodeHolder node, int amountOfInputs, Func<IValueNode[], T> func, bool nodeShowed = true)
            : base(name, node, amountOfInputs, nodeShowed)
        {
            _func = func;
        }

        public override void UpdateValue()
        {
            if (_func != null)
            {
                List<IValueNode> inputs = new List<IValueNode>();

                for (int i = 0; i < InputPins.Length; i++)
                {
                    if (InputPins[i].IncomingNode != null && InputPins[i].IncomingNode is IValueNode valueNode)
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