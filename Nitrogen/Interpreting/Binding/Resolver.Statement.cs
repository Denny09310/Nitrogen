using Nitrogen.Syntax.Abstractions;
using Nitrogen.Syntax.Statements;

namespace Nitrogen.Interpreting.Binding;

internal partial class Resolver
{
    public void Resolve(IStatement statement)
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
            case ClassStatement function: Resolve(function); break;

            default: break;
        }
    }

    private void Resolve(PrintStatement statement)
    {
        Resolve(statement.Expression);
    }

    private void Resolve(VarStatement statement)
    {
        Define(statement.Name);
        if (statement.Initializer is not null)
        {
            Resolve(statement.Initializer);
        }
        Declare(statement.Name);
    }

    private void Resolve(ExpressionStatement statement)
    {
        Resolve(statement.Expression);
    }

    private void Resolve(WhileStatement statement)
    {
        ResolveLoop(statement, LoopType.While);
    }

    private void Resolve(ForStatement statement)
    {
        ResolveLoop(statement, LoopType.For);
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
        Define(statement.Name);
        Declare(statement.Name);

        ResolveFunction(statement, FunctionType.Function);
    }

    private void Resolve(ClassStatement statement)
    {
        var enclosing = _currentClass;
        _currentClass = ClassType.Class;

        Define(statement.Name);
        Declare(statement.Name);

        BeginScope();

        AddVariable("this");

        foreach (var method in statement.Methods)
        {
            ResolveFunction(method, FunctionType.Method);
        }

        EndScope();

        _currentClass = enclosing;
    }
}