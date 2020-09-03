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

                ParameterExpression[] parameters = GetParametersForExpression(expr);
                foreach (var parameter in parameters)
                {
                    values.Add(NodePins[parameter.Name].Value);
                }

                Delegate func = Expression.Lambda(expr, parameters).Compile();

                nodePin.Value = func.DynamicInvoke(values.ToArray());
            }
        }

        private static ParameterExpression[] GetParametersForExpression(Expression expr)
        {
            List<ParameterExpression> parameters = new List<ParameterExpression>();

            if (expr is BinaryExpression binaryExpression)
            {
                parameters.AddRange(GetParametersForBinaryExpression(binaryExpression));
            }
            if (expr is MethodCallExpression methodExpression)
            {
                parameters.AddRange(GetParametersForMethodExpression(methodExpression));
            }
            if (expr is ParameterExpression paramExpr)
            {
                parameters.Add(paramExpr);
            }

            return parameters.ToArray();
        }

        private static ParameterExpression[] GetParametersForBinaryExpression(BinaryExpression binaryExpression)
        {
            List<ParameterExpression> parameters = new List<ParameterExpression>();

            parameters.AddRange(GetParametersForExpression(binaryExpression.Left));
            parameters.AddRange(GetParametersForExpression(binaryExpression.Right));

            return parameters.ToArray();
        }

        private static ParameterExpression[] GetParametersForMethodExpression(MethodCallExpression methodExpression)
        {
            List<ParameterExpression> parameters = new List<ParameterExpression>();

            foreach (var argExpr in methodExpression.Arguments)
            {
                parameters.AddRange(GetParametersForExpression(argExpr));
            }

            return parameters.ToArray();
        }

    }
}
