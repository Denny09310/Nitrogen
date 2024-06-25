﻿using Nitrogen.Syntax.Abstractions;
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
        Resolve(statement.Condition);
        Resolve(statement.Body);
    }

    private void Resolve(ForStatement statement)
    {
        if (statement.Initialization is not null) Resolve(statement.Initialization);
        Resolve(statement.Condition);
        Resolve(statement.Body);
        if (statement.Increment is not null) Resolve(statement.Increment);
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

    private void ResolveFunction(FunctionStatement statement, FunctionType type)
    {
        (var enclosingFunction, _currentFunction) = (_currentFunction, type);

        BeginScope();

        foreach (var argument in statement.Arguments)
        {
            if (argument is AssignmentExpression assignment)
            {
                Define(assignment.Name);
                Declare(assignment.Name);
            }
            else if (argument is IdentifierExpression identifier)
            {
                Define(identifier.Name);
                Declare(identifier.Name);
            }
        }

        Resolve(statement.Body);

        EndScope();

        _currentFunction = enclosingFunction;
    }
}