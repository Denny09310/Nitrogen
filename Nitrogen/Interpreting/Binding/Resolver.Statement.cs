using Nitrogen.Exceptions;
using Nitrogen.Syntax.Abstractions;
using Nitrogen.Syntax.Expressions;
using Nitrogen.Syntax.Statements;

namespace Nitrogen.Interpreting.Binding;

internal partial class Resolver
{
    private void Resolve(IStatement statement)
    {
        switch (statement)
        {
            case PrintStatement print: Resolve(print); break;
            case VarStatement variable: Resolve(variable); break;
            case ExpressionStatement expression: Resolve(expression); break;
            case WhileStatement @while: Resolve(@while); break;
            case ForStatement @for: Resolve(@for); break;
            case BlockStatement block: Resolve(block); break;
            case IfStatement @if: Resolve(@if); break;
            case FunctionStatement function: Resolve(function); break;

            default: break;
        }
    }

    private void Resolve(PrintStatement statement)
    {
        Resolve(statement.Expression);
    }

    private void Resolve(VarStatement statement)
    {
        Declare(statement.Name);
        if (statement.Initializer is not null)
        {
            Resolve(statement.Initializer);
        }
        Define(statement.Name);
    }

    private void Resolve(ExpressionStatement statement)
    {
        Resolve(statement.Expression);
    }

    private void Resolve(WhileStatement statement)
    {
        (var enclosing, _currentLoop) = (_currentLoop, new Loop { Type = LoopType.While });

        Resolve(statement.Condition);
        Resolve(statement.Body);

        if (!_currentLoop.CanExit)
        {
            _errors.Add(new BindingException(ExceptionLevel.Warning, statement.Keyword, "Possible infinite loop detected."));
        }

        _currentLoop = enclosing;
    }

    private void Resolve(ForStatement statement)
    {
        (var enclosing, _currentLoop) = (_currentLoop, new Loop { Type = LoopType.For });

        if (statement.Initialization is not null) Resolve(statement.Initialization);
        Resolve(statement.Condition);
        Resolve(statement.Body);

        if (statement.Increment is not null)
        {
            Resolve(statement.Increment);
        }
        else if (!_currentLoop.CanExit)
        {
            _errors.Add(new BindingException(ExceptionLevel.Warning, statement.Keyword, "Possible infinite loop detected."));
        }

        _currentLoop = enclosing;
    }

    private void Resolve(BlockStatement statement)
    {
        BeginScope();
        foreach (var stmt in statement.Statements)
        {
            Resolve(stmt);
        }
        EndScope();
    }

    private void Resolve(IfStatement statement)
    {
        Resolve(statement.Condition);
        Resolve(statement.Then);
        if (statement.Else is not null) Resolve(statement.Else);
    }

    private void Resolve(FunctionStatement statement)
    {
        Declare(statement.Name);
        Define(statement.Name);

        ResolveFunction(statement, FunctionType.Function);
    }
}