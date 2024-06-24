using Nitrogen.Syntax;
using Nitrogen.Syntax.Abstractions;
using Nitrogen.Syntax.Expressions;

using System.Diagnostics;

namespace Nitrogen.Interpreting;

internal partial class Interpreter
{
    private object? Evaluate(IExpression expr) => expr switch
    {
        BinaryExpression expression => Evaluate(expression),
        LiteralExpression expression => expression.Literal,
        _ => throw new UnreachableException($"Expression {expr.GetType()} not recognized.")
    };

    private object? Evaluate(BinaryExpression expression)
    {
        var left = new EvaluationResult(Evaluate(expression.Left));
        var right = new EvaluationResult(Evaluate(expression.Right));

        var @operator = expression.Operator;

        return @operator.Kind switch
        {
            TokenKind.Plus => left + right,
            TokenKind.Minus => left - right,
            TokenKind.Star => left * right,
            TokenKind.Slash => left / right,
            _ => throw new UnreachableException($"{@operator.Lexeme} not supported.")
        };
    }
}