using Nodum.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Nodum.Calc
{
    [Serializable]
    [Node(Group = "Calc")]
    public class RegexMathNode : Node
    {
        public override bool IsEditable => false;
        [NodePin(IsInvokeUpdatePins = true, CanSetValue = true)] public string RegexOperation { get; set; }
        [Output] public double Result { get; set; }

        private string _oldOperation;

        public RegexMathNode(string name = "RegexMathNode") : base(name)
        {
        }

        public override void UpdatePins()
        {
            if (!string.IsNullOrEmpty(RegexOperation) && RegexOperation != _oldOperation)
            {
                Expression expression = ExpressionParser.ParseExpression(RegexOperation);

                ParameterExpression[] parameters = expression.GetParameters();

                if (!string.IsNullOrEmpty(_oldOperation))
                {
                    ParameterExpression[] oldParameters = ExpressionParser.ParseExpression(_oldOperation).GetParameters();

                    foreach (var oldParameter in oldParameters)
                    {
                        if (!parameters.Any(p => p == oldParameter))
                        {
                            ProtectedRemoveNodePin(oldParameter.Name);
                        }
                    }
                }

                foreach (var parameter in parameters)
                {
                    if (!NodePins.ContainsKey(parameter.Name))
                    {
                        NodePin inputNodePin = NodePinBuilder.BuildNodePin(parameter.Name, this, typeof(double), new NodePinOptions() { IsInput = true, IsInvokeUpdate = true });

                        ProtectedTryAddNodePin(inputNodePin);
                    }
                }

                _oldOperation = RegexOperation;

                OnUpdatePins?.Invoke();
            }
        }

        public override void UpdateValue()
        {
            if (!string.IsNullOrEmpty(RegexOperation))
            {
                Expression expression = ExpressionParser.ParseExpression(RegexOperation);

                ParameterExpression[] parameters = expression.GetParameters();

                List<object> values = new List<object>();

                foreach (var parameter in parameters)
                {
                    values.Add(NodePins[parameter.Name].Value);
                }

                LambdaExpression lambda = Expression.Lambda(expression, parameters);

                Delegate func = lambda.Compile();

                Result = (double)func.DynamicInvoke(values.ToArray());

            }
        }

        public override string GetStringForNodePin(NodePin nodePin)
        {
            if (nodePin.Node == this)
            {
                if (nodePin.Name == "Result")
                {
                    if (!string.IsNullOrEmpty(RegexOperation))
                    {
                        return RegexOperation;
                    }
                }
            }
            return base.GetStringForNodePin(nodePin);
        }

        public override Expression GetExpressionForNodePin(NodePin nodePin)
        {
            if (nodePin.Node == this)
            {
                if (nodePin.Name == "Result")
                {
                    Expression expression = ExpressionParser.ParseExpression(RegexOperation);

                    ParameterExpressionChanger parameterChanger = new ParameterExpressionChanger();

                    return parameterChanger.Modify(expression, this);
                }
            }
            return base.GetExpressionForNodePin(nodePin);
        }
    }
}
