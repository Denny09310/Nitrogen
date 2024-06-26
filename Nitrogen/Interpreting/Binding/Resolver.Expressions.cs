using Nitrogen.Exceptions;
using Nitrogen.Syntax.Abstractions;
using Nitrogen.Syntax.Expressions;
using Nitrogen.Syntax.Statements;

namespace Nitrogen.Interpreting.Binding;

internal partial class Resolver
{
    private void Resolve(IExpression expression)
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

            case BreakExpression:
                if (_currentLoop is not null) _currentLoop.CanExit = true;
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
        if (!_variables.TryGetValue(identifier, out var variable))
        {
            _errors.Add(new(ExceptionLevel.Error, expression.Name, $"Undefined variable '{expression.Name.Lexeme}'."));
            return;
        }

        if (!variable.Declared)
        {
            _errors.Add(new(ExceptionLevel.Error, expression.Name, "Cannot read local variable in its own initializer."));
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
            _errors.Add(new(ExceptionLevel.Error, expression.Keyword, "Can't return from top-level statemet."));
        }

        if (_currentLoop is not null)
        {
            _currentLoop.CanExit = true;
        }

        if (expression.Value is not null) Resolve(expression.Value);
    }
}