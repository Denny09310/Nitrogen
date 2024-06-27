using Nitrogen.Exceptions;
using Nitrogen.Syntax.Abstractions;
using Nitrogen.Syntax.Expressions;
using Nitrogen.Syntax.Statements;

namespace Nitrogen.Interpreting.Binding;

internal class FunctionResolver(Resolver resolver)
{
    private FunctionType _current;

    public void Resolve(ReturnExpression expression)
    {
        if (_current is FunctionType.None)
        {
            resolver.Error(ExceptionLevel.Error, expression.Keyword, "Can't return from top-level statemet.");
        }

        if (expression.Value is not null) resolver.Resolve(expression.Value);
    }

    public void ResolveFunction(FunctionStatement statement, FunctionType type)
    {
        var enclosing = _current;
        _current = type;

        resolver.BeginScope();

        ResolveArguments(statement.Arguments);
        resolver.Resolve(statement.Body);

        resolver.EndScope();

        _current = enclosing;
    }

    private void ResolveArguments(IList<IExpression> arguments)
    {
        foreach (var argument in arguments)
        {
            if (argument is AssignmentExpression assignment)
            {
                resolver.Declare(assignment.Name);
                resolver.Define(assignment.Name);
            }
            else if (argument is IdentifierExpression identifier)
            {
                resolver.Declare(identifier.Name);
                resolver.Define(identifier.Name);
            }
        }
    }
}