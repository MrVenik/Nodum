using Nodum.Node;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NodumVisualCalculator.Data
{
    public class MathNode : Node
    {
        public enum MathType
        {
            Add,
            Subtract,
            Multiply,
            Divide,
            Remainder,
            Pow,
            Root,
        }

        [NodePin(IsInvokeUpdate = true, CanSetValue = true)] public MathType Operation;
        [Input] public double InputA { get; set; }
        [Input] public double InputB { get; set; }
        [Output] public double Result { get; set; }

        public override void UpdateValue()
        {
            Result = 0;
            switch (Operation)
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
                case MathType.Remainder:
                    if (InputB != 0)
                    {
                        Result = InputA % InputB;
                    }
                    break;
                case MathType.Pow:
                    Result = Math.Pow(InputA, InputB);
                    break;
                case MathType.Root:
                    Result = Math.Pow(InputA, 1.0 / InputB);
                    break;
            }
        }
    }

    public class RegexMathNode : Node
    {
        [NodePin(IsInvokeUpdatePins = true, CanSetValue = true)] public string RegexOperation { get; set; }
        [Output] public double Result { get; set; }

        private string _oldRegexOperation;
        private readonly MatchEvaluator _evaluator;

        public RegexMathNode()
        {
            _evaluator = new MatchEvaluator(MatchReplacer);
        }

        public override void UpdatePins()
        {
            if (!string.IsNullOrEmpty(RegexOperation) && RegexOperation != _oldRegexOperation)
            {
                Match[] matches = Regex.Matches(RegexOperation, @"\b[^\d]\w+").ToArray();

                if (!string.IsNullOrEmpty(_oldRegexOperation))
                {
                    Match[] oldMatches = Regex.Matches(_oldRegexOperation, @"\b[^\d]\w+").ToArray();
                    foreach (Match match in oldMatches)
                    {
                        if (!matches.Any(m => m.Value == match.Value))
                        {
                            RemoveNodePin(match.Value);
                        }
                    }
                }

                foreach (Match match in matches)
                {
                    if (!NodePins.ContainsKey(match.Value))
                    {
                        NodePin inputNodePin = NodeBuilder.BuildNodePin(match.Value, typeof(double), true, false, true);

                        TryAddNodePin(inputNodePin);
                    }
                }

                _oldRegexOperation = RegexOperation;
                OnUpdatePins?.Invoke();
            }
        }

        public override void UpdateValue()
        {
            if (!string.IsNullOrEmpty(RegexOperation))
            {
                string math = Regex.Replace(RegexOperation, @"\b[^\d]\w+", _evaluator).Trim();

                Result = Convert.ToDouble(new DataTable().Compute(math, ""));
            }
        }

        private string MatchReplacer(Match match)
        {
            if (NodePins.TryGetValue(match.Value, out NodePin input))
            {
                return input.Value.ToString();
            }
            return string.Empty;
        }
    }

    public class NumberNode : Node
    {
        [InputOutput] public double Number;
    }

    public class EmptyNode : Node
    {

    }

    public class VisualNode
    {
        public List<VisualNodePin> VisualNodePins { get; private set; } = new List<VisualNodePin>();

        public List<VisualNode> InternalVisualNodes { get; private set; } = new List<VisualNode>();

        public VisualNode(string name, VisualNode holder = null)
        {
            Holder = holder;
            Node = new EmptyNode() { Name = name };
            Editable = true;
        }

        public VisualNode(NodePin nodePin, VisualNode holder)
        {
            Holder = holder;
            Node = new EmptyNode() { Name = nodePin.Name };

            Editable = false;

            VisualNodePin visualNodePin = new VisualNodePin()
            {
                NodePin = nodePin,
                VisualNode = this,
                ElementId = $"{nodePin.Name}_{nodePin.Guid}",
                Showed = true,
                IsInternal = true
            };
            VisualNodePins.Add(visualNodePin);
        }

        public VisualNode(Node node, VisualNode holder)
        {
            Node = node;
            Holder = holder;

            Node.OnUpdatePins += () => ReAddNodePins(false);

            Editable = false;

            ReAddNodePins(true);
        }

        private void ReAddNodePins(bool firstLoad)
        {
            foreach (var pin in Node.AllNodePins)
            {
                if (!VisualNodePins.Any(vn => vn.NodePin == pin))
                {
                    VisualNodePin visualNodePin = new VisualNodePin()
                    {
                        NodePin = pin,
                        VisualNode = this,
                        ElementId = $"{pin.Name}_{pin.Guid}",
                        Showed = true
                    };
                    VisualNodePins.Add(visualNodePin);
                }
            }
            if (!firstLoad)
            {
                List<VisualNodePin> toRemove = new List<VisualNodePin>();
                foreach (var visualPin in VisualNodePins)
                {
                    if (!Node.AllNodePins.Contains(visualPin.NodePin))
                    {
                        toRemove.Add(visualPin);
                    }
                }
                foreach (var visualPin in toRemove)
                {
                    VisualNodePins.Remove(visualPin);
                }
            }
        }

        public List<NodePinConnection> IncomingConnections { get; private set; } = new List<NodePinConnection>();
        public List<NodePinConnection> OutgoingConnections { get; private set; } = new List<NodePinConnection>();

        public Node Node { get; private set; }
        public VisualNode Holder { get; private set; }
        public string Name
        {
            get => Node.Name;
            set
            {
                Node.Name = value;
            }
        }
        //public Guid Guid { get => Node.Guid; }
        public Position Position { get; set; } = new Position();
        public bool Editable { get; set; }
        public bool Showed { get; set; }
        public bool Focused { get; set; }
        public bool MenuShowed { get; set; }


        public void AddInputNodePin(Type pinType, string name = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = $"Input_{Node.AllInputNodePins.Count()}";
            }

            bool isInput = true;
            bool isOutput = false;
            bool isInvokeUpdate = true;

            NodePin nodePin = NodeBuilder.BuildNodePin(name, pinType, isInput, isOutput, isInvokeUpdate);

            AddVisualNodePin(nodePin);
        }

        public void AddOutputNodePin(Type pinType, string name = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = $"Output_{Node.AllOutputNodePins.Count()}";
            }

            bool isInput = false;
            bool isOutput = true;
            bool isInvokeUpdate = false;

            NodePin nodePin = NodeBuilder.BuildNodePin(name, pinType, isInput, isOutput, isInvokeUpdate);
            AddVisualNodePin(nodePin);
        }

        public void AddVisualNodePin(NodePin nodePin)
        {
            if (Node.TryAddNodePin(nodePin))
            {
                VisualNodePin visualNodePin = new VisualNodePin()
                {
                    NodePin = nodePin,
                    VisualNode = this,
                    ElementId = $"{nodePin.Name}_{nodePin.Guid}",
                    Showed = true
                };

                InternalVisualNodes.Add(new VisualNode(nodePin, this));

                VisualNodePins.Add(visualNodePin);
            }
        }

        public void AddNode(VisualNode visualNode)
        {
            InternalVisualNodes.Add(visualNode);
        }

        public void AddNodes(params VisualNode[] visualNodes)
        {
            foreach (var node in visualNodes)
            {
                AddNode(node);
            }
        }

        public void RemoveAllNodes()
        {
            foreach (var node in InternalVisualNodes)
            {
                node.Close();
            }
            InternalVisualNodes.Clear();
        }

        public void RemoveNode(VisualNode visualNode)
        {
            visualNode.Close();

            InternalVisualNodes.Remove(visualNode);
        }

        public void Close()
        {
            CloseIncomingConnections();
            CloseOutgoingConnections();

            RemoveAllNodes();

            Node.Close();
        }

        private void CloseIncomingConnections()
        {
            for (int i = 0; i < IncomingConnections.Count; i++)
            {
                NodePinConnection item = IncomingConnections[i];
                item.CloseConnection();
            }
        }

        private void CloseOutgoingConnections()
        {
            for (int i = 0; i < OutgoingConnections.Count; i++)
            {
                NodePinConnection item = OutgoingConnections[i];
                item.CloseConnection();
            }
        }
    }
}
