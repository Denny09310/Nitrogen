﻿using Nitrogen.Exceptions;
using Nitrogen.Extensions;
using Nitrogen.Interpreting.Declarations;
using Nitrogen.Syntax;
using Nitrogen.Syntax.Abstractions;
using Nitrogen.Syntax.Expressions;

using System.Diagnostics;

namespace Nitrogen.Interpreting;

public partial class Interpreter
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
        LiteralExpression expression => expression.Literal.ToInternal(),
        ReturnExpression expression => Evaluate(expression),
        GetterExpression expression => Evaluate(expression),
        SetterExpression expression => Evaluate(expression),
        ThisExpression expression => Evaluate(expression),
        SuperExpression expression => Evaluate(expression),
        ArrayExpression expression => Evaluate(expression),
        IndexExpression expression => Evaluate(expression),
        PrefixExpression expression => Evaluate(expression),
        PostfixExpression expression => Evaluate(expression),
        BreakExpression => throw new BreakException(),
        ContinueExpression => throw new ContinueException(),
        _ => throw new UnreachableException($"Expression {expr.GetType()} not recognized.")
    };

    private object? Evaluate(AssignmentExpression expression)
    {
        var value = Evaluate(expression.Value);

        if (!_locals.TryGetValue(expression, out var distance))
        {
            throw new RuntimeException(expression.Name, "Can't assign variable in global scope.");
        }

        _environment.AssignAt(distance, expression.Name, value);

        return value;
    }

    private object? Evaluate(BinaryExpression expression)
    {
        var left = new Evaluation(Evaluate(expression.Left));
        var right = new Evaluation(Evaluate(expression.Right));

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
        bool left = new Evaluation(expression.Left);

        if (expression.Operator is { Kind: TokenKind.Or or TokenKind.PipePipe })
        {
            if (left) return left;
        }
        else
        {
            if (!left) return left;
        }

        bool right = new Evaluation(Evaluate(expression.Right));
        return right;
    }

    private object? Evaluate(UnaryExpression expression)
    {
        var value = new Evaluation(Evaluate(expression.Expression));
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
        var function = Evaluate(expression.Target);

        if (function is not ICallable callable)
        {
            throw new RuntimeException(expression.Paren, $"Call target is not callable.");
        }

        var parameters = expression.Parameters.Select(Evaluate).ToArray();
        callable.Arity(parameters);

        return callable.Call(this, parameters);
    }

    private object? Evaluate(IdentifierExpression expression)
    {
        return LookupVariable(expression, expression.Name);
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

    private object? Evaluate(GetterExpression expression)
    {
        var target = Evaluate(expression.Expression);

        if (target is IInstance instance)
        {
            return instance.Get(expression.Name);
        }

        throw new RuntimeException(expression.Name, "Only instances have properties.");
    }

    private object? Evaluate(SetterExpression expression)
    {
        var target = Evaluate(expression.Object);

        if (target is not IInstance instance)
        {
            throw new RuntimeException(expression.Name, "Only instances have properties.");
        }

        var value = Evaluate(expression.Value);
        instance.Set(expression.Name, value);

        return value;
    }

    private object? Evaluate(ThisExpression expression)
    {
        return LookupVariable(expression, expression.Keyword, global: false);
    }

    private object? Evaluate(SuperExpression expression)
    {
        if (!_locals.TryGetValue(expression, out var distance))
        {
            throw new RuntimeException(expression.Keyword, "Super class not found in this scope.");
        }

        if (_environment.GetAt("super", distance) is not ClassDeclaration superclass)
        {
            throw new RuntimeException(expression.Keyword, "The class has no super class");
        }

        if (_environment.GetAt("this", distance - 1) is not ClassInstance instance)
        {
            throw new RuntimeException(expression.Keyword, "Invalid instance getter.");
        }

        if (expression.Type is SuperType.Accessor)
        {
            if (superclass.FindMethod(expression.Member.Lexeme) is not FunctionDeclaration method)
            {
                throw new RuntimeException(expression.Keyword, $"Undefined property '{expression.Member.Lexeme}'.");
            }

            return method.Bind(instance);
        }

        if (superclass.FindMethod("constructor") is not FunctionDeclaration constructor)
        {
            throw new RuntimeException(expression.Keyword, "The class has no super class");
        }

        var args = expression.Parameters.Select(Evaluate).ToArray();
        return constructor.Bind(instance).Call(this, args);
    }

    private object? Evaluate(ArrayExpression expression)
    {
        return expression.Items.Select(Evaluate).ToInternal();
    }

    private object? Evaluate(IndexExpression expression)
    {
        var array = Evaluate(expression.Array).Unwrap();
        var index = Evaluate(expression.Index);

        // Check that the array is indeed an array
        if (array is not object?[] value)
        {
            throw new RuntimeException(expression.Bracket, "Target is not an array.");
        }

        // Ensure the index is an integer
        if (index is not double @double)
        {
            throw new RuntimeException(expression.Bracket, "Array index must be an integer.");
        }

        // Check bounds
        if (@double < 0 || @double >= value.Length)
        {
            throw new RuntimeException(expression.Bracket, "Array index out of bounds.");
        }

        return value[(int)@double];
    }

    private object? Evaluate(PrefixExpression expression)
    {
        var identifier = new Evaluation(Evaluate(expression.Identifier));
        var @operator = expression.Operator;

        var currentValue = @operator.Kind switch
        {
            TokenKind.PlusPlus => identifier + Evaluation.One,
            TokenKind.MinusMinus => identifier - Evaluation.One,

            _ => throw new RuntimeException(@operator, $"{@operator.Lexeme} not supported.")
        };

        if (expression.Identifier is IdentifierExpression identifierExpr)
        {
            _environment.Assign(identifierExpr.Name, currentValue);
        }

        return currentValue;
    }

    private object? Evaluate(PostfixExpression expression)
    {
        var identifier = new Evaluation(Evaluate(expression.Identifier));
        var @operator = expression.Operator;

        if (@operator.Kind is not TokenKind.PlusPlus and not TokenKind.MinusMinus)
        {
            throw new RuntimeException(@operator, $"{@operator.Lexeme} not supported as postfix.");
        }

        var currentValue = new Evaluation(identifier.Value);

        switch (@operator.Kind)
        {
            case TokenKind.PlusPlus:
                identifier = new Evaluation(currentValue + Evaluation.One);
                break;

            case TokenKind.MinusMinus:
                identifier = new Evaluation(currentValue - Evaluation.One);
                break;
        }

        if (expression.Identifier is IdentifierExpression identifierExpr)
        {
            _environment.Assign(identifierExpr.Name, identifier.Value);
        }

        return currentValue;
    }
}