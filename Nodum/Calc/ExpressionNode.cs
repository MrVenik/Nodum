using Nodum.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Nodum.Calc
{
    [Serializable]
    [Node(NodeCacherIgnore = true)]
    public class ExpressionNode : Node
    {
        private Node _parentNode;

        public override bool IsEditable => false;

        public ExpressionNode(Node node, string name = "ExpressionNode") : base(name)
        {
            _parentNode = node;

            node.OnNodeChanged += ChangeNode;

            foreach (var pin in node.AllNodePins)
            {
                NodePin nodePin = NodePinBuilder.CloneNodePin(pin, this);

                ProtectedTryAddNodePin(nodePin);
            }
        }

        public void ChangeNode()
        {
            foreach (var pin in _parentNode.AllNodePins)
            {
                NodePin nodePin = NodePinBuilder.CloneNodePin(pin, this);

                if (!ProtectedTryAddNodePin(nodePin))
                {
                    NodePin oldIncomingPin = NodePins[nodePin.Name].IncomingNodePin;
                    object oldValue = NodePins[nodePin.Name].Value;

                    List<NodePin> oldOutgoingNodePins = new List<NodePin>();

                    foreach (var outPin in NodePins[nodePin.Name].OutgoingNodePins)
                    {
                        oldOutgoingNodePins.Add(outPin);
                    }

                    ProtectedSetNodePin(nodePin);

                    foreach (var outPin in oldOutgoingNodePins)
                    {
                        outPin.AddIncomingNodePin(nodePin);
                    }

                    if (oldIncomingPin != null)
                    {
                        nodePin.AddIncomingNodePin(oldIncomingPin);
                    }
                    else
                    {
                        nodePin.Value = oldValue;
                    }
                }
            }

            foreach (var pin in AllNodePins)
            {
                if (_parentNode.AllNodePins.FirstOrDefault(p => p.Name == pin.Name) == null)
                {
                    ProtectedRemoveNodePin(pin);
                }
            }

            Update();
        }

        public override void UpdateValue()
        {
            foreach (var nodePin in AllOutputNodePins)
            {
                Expression expr = _parentNode.GetExpressionForNodePin(_parentNode.NodePins[nodePin.Name]);

                List<object> values = new List<object>();

                ParameterExpression[] parameters = expr.GetParameters();
                foreach (var parameter in parameters)
                {
                    values.Add(NodePins[parameter.Name].Value);
                }

                LambdaExpression lambda = Expression.Lambda(expr, parameters);

                Delegate func = lambda.Compile();

                nodePin.Value = func.DynamicInvoke(values.ToArray());
            }
        }

        public override Expression GetExpressionForNodePin(NodePin nodePin)
        {
            if (nodePin.Node == this)
            {
                if (nodePin.IsOutput)
                {
                    ParameterExpressionChanger parameterChanger = new ParameterExpressionChanger();

                    return parameterChanger.Modify(_parentNode.GetExpressionForNodePin(_parentNode.NodePins[nodePin.Name]), this);
                }
            }
            return base.GetExpressionForNodePin(nodePin);
        }
    }
}
