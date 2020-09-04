using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nodum.Core
{
    public static class ExpressionExtensions
    {
        public static ParameterExpression[] GetParameters(this Expression expr)
        {
            List<ParameterExpression> parameters = new List<ParameterExpression>();

            if (expr is BinaryExpression binaryExpression)
            {
                var prmts = binaryExpression.GetParameters();

                foreach (var parameter in prmts)
                {
                    if (!parameters.Contains(parameter))
                    {
                        parameters.Add(parameter);
                    }
                }
            }
            if (expr is MethodCallExpression methodExpression)
            {
                var prmts = methodExpression.GetParameters();

                foreach (var parameter in prmts)
                {
                    if (!parameters.Contains(parameter))
                    {
                        parameters.Add(parameter);
                    }
                }
            }
            if (expr is UnaryExpression unaryExpression)
            {
                var prmts = unaryExpression.GetParameters();

                foreach (var parameter in prmts)
                {
                    if (!parameters.Contains(parameter))
                    {
                        parameters.Add(parameter);
                    }
                }
            }
            if (expr is ConditionalExpression conditionalExpression)
            {
                var prmts = conditionalExpression.GetParameters();

                foreach (var parameter in prmts)
                {
                    if (!parameters.Contains(parameter))
                    {
                        parameters.Add(parameter);
                    }
                }
            }
            if (expr is ParameterExpression paramExpr)
            {
                parameters.Add(paramExpr);
            }

            return parameters.ToArray();
        }

        public static ParameterExpression[] GetParameters(this ConditionalExpression conditionalExpression)
        {
            List<ParameterExpression> parameters = new List<ParameterExpression>();

            parameters.AddRange(GetParameters(conditionalExpression.Test));
            parameters.AddRange(GetParameters(conditionalExpression.IfTrue));
            parameters.AddRange(GetParameters(conditionalExpression.IfFalse));

            return parameters.ToArray();
        }

        public static ParameterExpression[] GetParameters(this BinaryExpression binaryExpression)
        {
            List<ParameterExpression> parameters = new List<ParameterExpression>();

            parameters.AddRange(GetParameters(binaryExpression.Left));
            parameters.AddRange(GetParameters(binaryExpression.Right));

            return parameters.ToArray();
        }

        public static ParameterExpression[] GetParameters(this UnaryExpression unaryExpression)
        {
            List<ParameterExpression> parameters = new List<ParameterExpression>();

            parameters.AddRange(GetParameters(unaryExpression.Operand));

            return parameters.ToArray();
        }

        public static ParameterExpression[] GetParameters(this MethodCallExpression methodExpression)
        {
            List<ParameterExpression> parameters = new List<ParameterExpression>();

            foreach (var argExpr in methodExpression.Arguments)
            {
                parameters.AddRange(GetParameters(argExpr));
            }

            return parameters.ToArray();
        }
    }
}
