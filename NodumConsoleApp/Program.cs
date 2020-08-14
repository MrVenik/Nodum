using System;
using Nodum.Node;

namespace NodumConsoleApp
{
    public class InputDoubleNode : SingleInputValueNode<double>
    {
        public InputDoubleNode(string name, double value = default)
            : base(name, value)
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

        public DoubleAddFuncNode(string name)
            : base(name, 2, Add)
        {
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            InputDoubleNode input1 = new InputDoubleNode("Input1", 2);
            InputDoubleNode input2 = new InputDoubleNode("Input2", 2);

            DoubleAddFuncNode addFuncNode = new DoubleAddFuncNode("Result");
            addFuncNode.AddIncomingNode(input1, 0);
            addFuncNode.AddIncomingNode(input2, 1);

            Console.WriteLine($"{input1.Name} = {input1.Value}");
            Console.WriteLine($"{input2.Name} = {input2.Value}");
            Console.WriteLine($"{addFuncNode.Name} = {addFuncNode.Value}");
        }
    }
}
