using Nodum.Core;
using System.Linq.Expressions;

namespace Nodum.Calc
{
    public class ParameterExpressionChanger : ExpressionVisitor
    {
        private Node _node;
        public Expression Modify(Expression expression, Node node)
        {
            _node = node;
            return Visit(expression);
        }

        public override Expression Visit(Expression b)
        {
            if (b.NodeType == ExpressionType.Parameter)
            {
                return _node.GetExpressionForNodePin(_node.NodePins[(b as ParameterExpression).Name]);
            }

            return base.Visit(b);
        }
    }
}
