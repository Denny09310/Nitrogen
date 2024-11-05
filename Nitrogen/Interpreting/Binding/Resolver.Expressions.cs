﻿using Nitrogen.Exceptions;
using Nitrogen.Syntax.Abstractions;
using Nitrogen.Syntax.Expressions;
using Nitrogen.Syntax.Statements;
using System.Xml.Linq;

namespace Nitrogen.Interpreting.Binding;

public partial class Resolver
{
    public void Resolve(IExpression expression)
    {
        switch (expression)
        {
            case AssignmentExpression assignment: Resolve(assignment); break;
            case IdentifierExpression identifier: Resolve(identifier); break;
            case BinaryExpression binary: Resolve(binary); break;
            case LogicalExpression logical: Resolve(logical); break;
            case UnaryExpression unary: Resolve(unary); break;
            case GroupingExpression grouping: Resolve(grouping); break;
            case CallExpression call: Resolve(call); break;
            case ReturnExpression @return: Resolve(@return); break;
            case GetterExpression getter: Resolve(getter); break;
            case SetterExpression setter: Resolve(setter); break;
            case ThisExpression setter: Resolve(setter); break;
            case SuperExpression setter: Resolve(setter); break;

            case BreakExpression:
                if (_currentLoop != null) _currentLoop.Infinite = false;
                break;

            case LiteralExpression or ContinueExpression:
            default: break;
        }
    }

    private void Resolve(AssignmentExpression expression)
    {
        Resolve(expression.Value);
        ResolveLocal(expression, expression.Name);
    }

    private void Resolve(IdentifierExpression expression)
    {
        int identifier = expression.Name.Lexeme.GetHashCode();

        Variable? variable = null;

        foreach (var scope in _scopes)
        {
            if (scope.TryGetValue(identifier, out variable)) break;
        }

        if (variable is null)
        {
            Report(ExceptionLevel.Error, expression.Name, $"Undefined variable '{expression.Name.Lexeme}'.");
            return;
        }

        if (!variable.Declared)
        {
            Report(ExceptionLevel.Error, expression.Name, "Cannot read local variable in its own initializer.");
            return;
        }

        ResolveLocal(expression, expression.Name);
    }

    private void Resolve(BinaryExpression statement)
    {
        Resolve(statement.Left);
        Resolve(statement.Right);
    }

    private void Resolve(LogicalExpression expression)
    {
        Resolve(expression.Left);
        Resolve(expression.Right);
    }

    private void Resolve(UnaryExpression expression)
    {
        Resolve(expression.Expression);
    }

    private void Resolve(GroupingExpression expression)
    {
        Resolve(expression.Expression);
    }

    private void Resolve(CallExpression expression)
    {
        if (expression.Target is GetterExpression getter && getter.Name.Lexeme is "constructor")
        {
            Report(ExceptionLevel.Error, getter.Name, "Cannot call class contructor explicitly.");
        }

        Resolve(expression.Target);

        foreach (var parameter in expression.Parameters)
        {
            Resolve(parameter);
        }
    }

    private void Resolve(ReturnExpression expression)
    {
        if (_currentFunction is FunctionType.None)
        {
            Report(ExceptionLevel.Error, expression.Keyword, "Can't return from top-level statemet.");
        }

        if (expression.Value is not null)
        {
            if (_currentFunction is FunctionType.Constructor)
            {
                Report(ExceptionLevel.Error, expression.Keyword, "Can't return a value from constructor.");
            }

            Resolve(expression.Value);
        }
    }

    private void Resolve(GetterExpression expression)
    {
        Resolve(expression.Expression);
    }

    private void Resolve(SetterExpression expression)
    {
        Resolve(expression.Object);
        Resolve(expression.Value);
    }

    private void Resolve(ThisExpression expression)
    {
        if (_currentClass is ClassType.None)
        {
            Report(ExceptionLevel.Error, expression.Keyword, "Can't use 'this' outside of a class.");
            return;
        }

        ResolveLocal(expression, expression.Keyword);
    }

    private void Resolve(SuperExpression expression)
    {
        if (_currentClass is ClassType.None)
        {
            Report(ExceptionLevel.Error, expression.Keyword, "Can't use 'super' outside of a class.");
            return;
        }
        else if (_currentClass is not ClassType.Subclass)
        {
            Report(ExceptionLevel.Error, expression.Keyword, "Can't use 'super' in a class with no superclass.");
            return;
        }

        if (expression.Type is SuperType.Constructor)
        {
            foreach (var parameter in expression.Parameters)
            {
                Resolve(parameter);
            }
        }

        ResolveLocal(expression, expression.Keyword);
    }
}