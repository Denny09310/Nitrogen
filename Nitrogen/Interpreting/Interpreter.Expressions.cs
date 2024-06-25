using Nitrogen.Exceptions;
using Nitrogen.Interpreting.Declarations;
using Nitrogen.Syntax;
using Nitrogen.Syntax.Abstractions;
using Nitrogen.Syntax.Expressions;
using System.Diagnostics;

namespace Nitrogen.Interpreting;

internal partial class Interpreter
{
    public object? Evaluate(IExpression expr) => expr switch
    {
        AssignmentExpression expression => Evaluate(expression),
        BinaryExpression expression => Evaluate(expression),
        LogicalExpression expression => Evaluate(expression),
        UnaryExpression expression => Evaluate(expression),
        GroupingExpression expression => Evaluate(expression),
        IdentifierExpression expression => Evaluate(expression),
        CallExpression expression => Evaluate(expression),
        LiteralExpression expression => expression.Literal,
        ReturnExpression expression => Evaluate(expression),
        BreakExpression => throw new BreakException(),
        ContinueExpression => throw new ContinueException(),
        _ => throw new UnreachableException($"Expression {expr.GetType()} not recognized.")
    };

    private object? Evaluate(AssignmentExpression expression)
    {
        var value = Evaluate(expression.Value);
        _environment.AssignVariable(expression.Target, value);
        return value;
    }

    private object? Evaluate(BinaryExpression expression)
    {
        var left = new EvaluationResult(Evaluate(expression.Left));
        var right = new EvaluationResult(Evaluate(expression.Right));

        var @operator = expression.Operator;

        return @operator.Kind switch
        {
            // Additive
            TokenKind.Plus => left + right,
            TokenKind.Minus => left - right,

            // Multiplicative
            TokenKind.Star => left * right,
            TokenKind.Slash => left / right,

            // Equality
            TokenKind.EqualEqual => left == right,
            TokenKind.BangEqual => left != right,

            // Comparison
            TokenKind.Less => left < right,
            TokenKind.LessEqual => left <= right,
            TokenKind.Greater => left > right,
            TokenKind.GreaterEqual => left >= right,

            _ => throw new RuntimeException(@operator, $"{@operator.Lexeme} not supported.")
        };
    }

    private object? Evaluate(LogicalExpression expression)
    {
        bool left = new EvaluationResult(expression.Left);

        if (expression.Operator is { Kind: TokenKind.Or or TokenKind.PipePipe })
        {
            if (left) return left;
        }
        else
        {
            if (!left) return left;
        }

        bool right = new EvaluationResult(Evaluate(expression.Right));
        return right;
    }

    private object? Evaluate(UnaryExpression expression)
    {
        var value = new EvaluationResult(Evaluate(expression.Expression));
        var @operator = expression.Operator;

        return @operator.Kind switch
        {
            TokenKind.Minus => -value,
            TokenKind.Bang => !value,

            _ => throw new RuntimeException(@operator, $"{@operator.Lexeme} not supported.")
        };
    }

    private object? Evaluate(GroupingExpression expression)
    {
        return Evaluate(expression.Expression);
    }

    private object? Evaluate(CallExpression expression)
    {
        var function = LookupVariable(expression.Name);

        if (function is not ICallable callable)
        {
            throw new RuntimeException(expression.Name, $"Variable '{expression.Name.Lexeme}' is not a function");
        }

        var parameters = expression.Parameters.Select(Evaluate).ToArray();
        callable.Arity(parameters);

        return callable.Call(this, parameters);
    }

    private object? Evaluate(IdentifierExpression expression)
    {
        return LookupVariable(expression.Name);
    }

    private object? Evaluate(ReturnExpression expression)
    {
        object? value = null;
        if (expression.Value is not null)
        {
            value = Evaluate(expression.Value);
        }

        throw new ReturnException(value);
    }
}