using Nitrogen.Syntax.Abstractions;
using Nitrogen.Syntax.Expressions;
using Nitrogen.Syntax.Statements;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Nitrogen.Parsing;

internal class AbstractSyntaxTree
{
    public string Print(List<IStatement> expressions)
    {
        StringBuilder builder = new();

        foreach (var expression in expressions)
        {
            builder.Append(Print(expression));
        }

        return builder.ToString();
    }

    private static string? Print(LiteralExpression expression) => expression.Literal switch
    {
        double @double => @double.ToString(CultureInfo.InvariantCulture),
        null => "nil",
        _ => expression.Literal.ToString()
    };

    private string? Print(IStatement stmt) => stmt switch
    {
        ExpressionStatement statement => Print(statement.Expression),
        PrintStatement => null,
        _ => throw new UnreachableException($"Unrecognized expression of type {stmt.GetType()}")
    };

    private string? Print(IExpression expr) => expr switch
    {
        BinaryExpression expression => Print(expression),
        LiteralExpression expression => Print(expression),
        _ => throw new UnreachableException($"Unrecognized expression of type {expr.GetType()}")
    };

    private string? Print(BinaryExpression expression)
    {
        return $"({Print(expression.Left)} {expression.Operator.Lexeme} {Print(expression.Right)})";
    }
}