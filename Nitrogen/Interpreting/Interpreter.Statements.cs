﻿using Nitrogen.Exceptions;
using Nitrogen.Interpreting.Declarations;
using Nitrogen.Syntax.Abstractions;
using Nitrogen.Syntax.Statements;
using System.Diagnostics;

namespace Nitrogen.Interpreting;

public partial class Interpreter
{
    public object? Execute(IStatement stmt) => stmt switch
    {
        ExpressionStatement statement => Execute(statement),
        PrintStatement statement => Execute(statement),
        WhileStatement statement => Execute(statement),
        ForStatement statement => Execute(statement),
        BlockStatement statement => Execute(statement),
        IfStatement statement => Execute(statement),
        FunctionStatement statement => Execute(statement),
        VarStatement statement => Execute(statement),
        _ => throw new UnreachableException($"Statement {stmt.GetType()} not recognized.")
    };

    public void ExecuteScoped(List<IStatement> statements, InterpreterEnvironment environment)
    {
        (var enclosing, _environment) = (_environment, environment);

        try
        {
            foreach (var statement in statements)
            {
                Execute(statement);
            }
        }
        finally
        {
            _environment = enclosing;
        }
    }

    private object? Execute(ExpressionStatement statement)
    {
        return Evaluate(statement.Expression);
    }

    private object? Execute(PrintStatement statement)
    {
        var value = new EvaluationResult(Evaluate(statement.Expression));
        Console.WriteLine(value.ToString());

        return null;
    }

    private object? Execute(WhileStatement statement)
    {
        ExecuteLoop(() => new EvaluationResult(Evaluate(statement.Condition)), statement.Body);
        return null;
    }

    private object? Execute(ForStatement statement)
    {
        if (statement.Initialization is not null) Execute(statement.Initialization);
        ExecuteLoop(() => new EvaluationResult(Evaluate(statement.Condition)), statement.Body, statement.Increment);
        return null;
    }

    private object? Execute(BlockStatement statement)
    {
        ExecuteScoped(statement.Statements, new InterpreterEnvironment(_environment));
        return null;
    }

    private object? Execute(IfStatement statement)
    {
        var condition = new EvaluationResult(Evaluate(statement.Condition));
        if (condition) Execute(statement.Then);
        else if (statement.Else is not null) Execute(statement.Else);

        return null;
    }

    private FunctionDeclaration Execute(FunctionStatement statement)
    {
        var function = new FunctionDeclaration(statement, _environment);
        _environment.Define(statement.Name, function);
        return function;
    }

    private object? Execute(VarStatement statement)
    {
        object? value = null;
        if (statement.Initializer is not null) value = Evaluate(statement.Initializer);
        _environment.Define(statement.Name, value);

        return null;
    }

    private void ExecuteLoop(Func<bool> condition, IStatement body, IExpression? increment = null)
    {
        while (condition())
        {
            try
            {
                Execute(body);
            }
            catch (BreakException)
            {
                break;
            }
            catch (ContinueException)
            {
                continue;
            }
            finally
            {
                if (increment != null)
                {
                    Evaluate(increment);
                }
            }
        }
    }
}