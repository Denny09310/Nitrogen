using Nitrogen.Syntax;
using Nitrogen.Syntax.Expressions;
using Nitrogen.Syntax.Expressions.Abstractions;

using System.Diagnostics;

namespace Nitrogen.Interpreting;

internal class Interpreter
{
    public void Evaluate(List<IExpression> expressions)
    {
        foreach (var expression in expressions)
        {
            Evaluate(expression);
        }
    }

    private object? Evaluate(IExpression expr) => expr switch
    {
        BinaryExpression expression => Evaluate(expression),
        LiteralExpression expression => Evaluate(expression),
        _ => throw new UnreachableException("Expression not recognized.")
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

    private object? Evaluate(LiteralExpression expression)
    {
        return expression.Literal;
    }
}