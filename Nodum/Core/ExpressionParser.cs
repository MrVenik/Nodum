using Sprache;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace Nodum.Core
{
    public static class ExpressionParser
    {
        public static Dictionary<string, ParameterExpression> Parameters;

        private static Parser<string> DecimalWithoutLeadingDigits =>
            from dot in Parse.Char('.')
            from fraction in Parse.Number
            select dot + fraction;

        private static Parser<string> DecimalWithLeadingDigits =>
            Parse.Number.Then(n => DecimalWithoutLeadingDigits.XOr(Parse.Return(string.Empty)).Select(f => n + f));

        private static Parser<string> Decimal =>
            DecimalWithLeadingDigits.XOr(DecimalWithoutLeadingDigits);

        private static ParameterExpression GetParameter(string str)
        {
            if (!Parameters.ContainsKey(str))
            {
                Parameters.Add(str, Expression.Parameter(typeof(double), str));
            }
            return Parameters[str];
        }

        private static Parser<Expression> Parameter =>
            Parse.Regex(@"\b[a-zA-Z]\w*").Select(s => GetParameter(s)).Named("Parameter");

        private static Parser<Expression> Constant =>
            Decimal.Select(x => Expression.Constant(double.Parse(x, CultureInfo.InvariantCulture))).Named("Constant");

        private static Parser<ExpressionType> Operator(string op, ExpressionType opType) =>
            Parse.String(op).Token().Return(opType);

        private static Parser<ExpressionType> Add =>
            Operator("+", ExpressionType.AddChecked);

        private static Parser<ExpressionType> Subtract =>
            Operator("-", ExpressionType.SubtractChecked);

        private static Parser<ExpressionType> Multiply =>
            Operator("*", ExpressionType.MultiplyChecked);

        private static Parser<ExpressionType> Divide =>
            Operator("/", ExpressionType.Divide);

        private static Parser<ExpressionType> Modulo =>
            Operator("%", ExpressionType.Modulo);

        private static Parser<ExpressionType> Power =>
            Operator("^", ExpressionType.Power);

        private static Parser<Expression> ExpressionInParentheses =>
            from lparen in Parse.Char('(')
            from expr in Expr
            from rparen in Parse.Char(')')
            select expr;

        private static Parser<Expression> Factor =>
            ExpressionInParentheses.XOr(Constant).XOr(Parameter);

        private static Parser<Expression> NegativeFactor =>
            from sign in Parse.Char('-')
            from factor in Factor
            select Expression.NegateChecked(factor);

        private static Parser<Expression> Operand =>
            (NegativeFactor.XOr(Factor)).Token();

        private static Parser<Expression> InnerTerm =>
            Parse.ChainRightOperator(Power, Operand, Expression.MakeBinary);

        private static Parser<Expression> Term =>
            Parse.ChainOperator(Multiply.Or(Divide).Or(Modulo), InnerTerm, Expression.MakeBinary);

        private static Parser<Expression> Expr =>
            Parse.ChainOperator(Add.Or(Subtract), Term, Expression.MakeBinary);

        private static Parser<Expression> FullExpr =>
            Expr.End();

        public static Expression ParseExpression(string text)
        {
            Parameters = new Dictionary<string, ParameterExpression>();
            return FullExpr.Parse(text);
        }
    }
}
