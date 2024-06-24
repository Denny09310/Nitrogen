using Nitrogen.Syntax.Abstractions;
using Nitrogen.Syntax.Statements;
using System.Diagnostics;

namespace Nitrogen.Interpreting;

internal partial class Interpreter
{
    public void Execute(List<IStatement> expressions)
    {
        foreach (var expression in expressions)
        {
            Execute(expression);
        }
    }

#pragma warning disable S3241 // Methods should not return values that are never used

    private object? Execute(IStatement stmt) => stmt switch
    {
        PrintStatement statement => Execute(statement),
        ExpressionStatement statement => Execute(statement),
        _ => throw new UnreachableException($"Statement {stmt.GetType()} not recognized.")
    };

#pragma warning restore S3241 // Methods should not return values that are never used

    private object? Execute(PrintStatement statement)
    {
        var value = new EvaluationResult(Evaluate(statement.Expression));
        Console.WriteLine(value);

        return null;
    }

    private object? Execute(ExpressionStatement statement)
    {
        return Evaluate(statement.Expression);
    }
}