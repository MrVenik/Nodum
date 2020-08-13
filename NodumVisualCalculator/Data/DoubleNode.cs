using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nodum.Node;

namespace NodumVisualCalculator.Data
{
    public class InputDoubleNode : SingleInputValueNode<double>
    {
        public InputDoubleNode(string name, INodeHolder node, bool nodePinShowed = true, bool outputNodePinShowed = true, double value = default)
            : base(name, node, nodePinShowed, outputNodePinShowed, value)
        {
        }
    }


    public class DoubleAddFuncNode : MultipleInputFuncNode<double>
    {
        private static double Add(IValueNode[] inputs)
        {
            if (inputs != null && inputs.Length == 2)
            {
                if (inputs[0] is ValueNode<double> input1 && inputs[1] is ValueNode<double> input2)
                {
                    return input1.Value + input2.Value;
                }
            }
            return default;
        }

        public DoubleAddFuncNode(string name, INodeHolder node, bool nodePinShowed = true)
            : base(name, node, 2, Add, nodePinShowed)
        {
        }
    }
}
