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

            case LiteralExpression or BreakExpression or ContinueExpression:
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
        if (_currentDepth != 0 && _variables.TryGetValue(expression.Name.Lexeme.GetHashCode(), out var variable) && !variable.Declared)
        {
            _errors.Add(new(ExceptionLevel.Error, expression.Name, "Cannot read local variable in its own initializer."));
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

        if (expression.Value is not null) Resolve(expression.Value);
    }
}