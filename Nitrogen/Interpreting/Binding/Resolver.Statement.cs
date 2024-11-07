using Nitrogen.Core.Exceptions;
using Nitrogen.Core.Syntax.Expressions;
using Nitrogen.Core.Syntax.Statements;
using Nitrogen.Core.Syntax.Statements.Abstractions;

namespace Nitrogen.Interpreting.Binding;

public partial class Resolver
{
    public void Resolve(IStatement statement)
    {
        switch (statement)
        {
            case VarStatement variable: Resolve(variable); break;
            case ExpressionStatement expression: Resolve(expression); break;
            case WhileStatement @while: Resolve(@while); break;
            case ForStatement @for: Resolve(@for); break;
            case BlockStatement block: Resolve(block); break;
            case IfStatement @if: Resolve(@if); break;
            case FunctionStatement function: Resolve(function); break;
            case ClassStatement function: Resolve(function); break;
            case ImportStatement import: Resolve(import); break;
            case TryStatement @try: Resolve(@try); break;
            case CatchStatement @catch: Resolve(@catch); break;
            case FinallyStatement @finally: Resolve(@finally); break;

            default: break;
        }
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
        Declare(statement.Name);
        Define(statement.Name);

        ResolveFunction(statement, FunctionType.Function);
    }

    private void Resolve(ClassStatement statement)
    {
        var enclosing = _currentClass;
        _currentClass = ClassType.Class;

        Declare(statement.Name);
        Define(statement.Name);

        if (statement.Superclass is not null)
        {
            if (statement.Name.Lexeme == statement.Superclass.Name.Lexeme)
            {
                Report(ExceptionLevel.Error, statement.Superclass.Name, "A class can't inherit from itself.");
            }

            _currentClass = ClassType.Subclass;
            Resolve(statement.Superclass);

            BeginScope();
            Define("super");
        }

        BeginScope();

        Define("this");

        foreach (var method in statement.Methods)
        {
            ResolveFunction(method, FunctionType.Method);
        }

        EndScope();

        if (statement.Superclass is not null)
        {
            EndScope();
        }

        _currentClass = enclosing;
    }

    private void Resolve(ImportStatement statement)
    {
        foreach (var import in statement.Imports)
        {
            var name = ((IdentifierExpression)import).Name;

            Declare(name);
            Define(name);

            ResolveLocal(import, name);
        }
    }

    private void Resolve(TryStatement statement)
    {
        Resolve(statement.Body);
        if (statement.Catch != null) Resolve(statement.Catch);
        if (statement.Finally != null) Resolve(statement.Finally);
    }

    private void Resolve(CatchStatement statement)
    {
        BeginScope();

        if (statement.Identifier != null)
        {
            var name = statement.Identifier.Name;

            Declare(name);
            Define(name);
        }

        Resolve(statement.Body);

        EndScope();
    }

    private void Resolve(FinallyStatement statement)
    {
        Resolve(statement.Body);
    }
}