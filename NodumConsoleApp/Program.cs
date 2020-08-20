using System;
using Nodum.Node;

namespace NodumConsoleApp
{
    public class MathNode : Node
    {
        public enum MathType
        {
            Add,
            Subtract,
            Multiply,
            Divide
        }

        [NodePin(IsInvokeUpdate = true)] public MathType Type;
        [Input] public int InputA;
        [Input] public int InputB;
        [Output] public int Result;

        public MathNode()
            : base()
        {
        }

        public override void UpdateValue()
        {
            Result = 0;
            switch (Type)
            {
                case MathType.Add:
                    Result = InputA + InputB;
                    break;
                case MathType.Subtract:
                    Result = InputA - InputB;
                    break;
                case MathType.Multiply:
                    Result = InputA * InputB;
                    break;
                case MathType.Divide:
                    if (InputB != 0)
                    {
                        Result = InputA / InputB;
                    }
                    break;
            }
        }
    }

    public class IntNode : Node
    {
        [InputOutput] public int Number;
    }

    class Program
    {
        static void Main(string[] args)
        {
            MathNode math = new MathNode();
            IntNode intNode = new IntNode();
            IntNode intNode1 = new IntNode();

            Debug(math);
            Debug(intNode);
            Debug(intNode1);

            intNode.AllNodePins[0].Value = 20;
            math.AllNodePins[1].AddIncomingNodePin(intNode.AllNodePins[0]);
            math.AllNodePins[2].AddIncomingNodePin(intNode.AllNodePins[0]);
            intNode1.AllNodePins[0].AddIncomingNodePin(math.AllNodePins[3]);

            Debug(math);
            Debug(intNode);
            Debug(intNode1);
        }

        private static void Debug(Node node)
        {
            foreach (var pin in node.AllNodePins)
            {
                Console.WriteLine($"{pin.GetType()}");
                Console.WriteLine($"{pin.Name}");
                Console.WriteLine($"{pin.Value}");
                Console.WriteLine($"{pin.ValueType}");
                Console.WriteLine();
            }
        }
    }
}
