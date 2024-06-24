using Nitrogen.Syntax.Expressions;
using Nitrogen.Syntax.Expressions.Abstractions;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Nitrogen.Parsing;

internal class AbstractSyntaxTree
{
    public string Print(List<IExpression> expressions)
    {
        StringBuilder builder = new();

        foreach (var expression in expressions)
        {
            builder.Append(Print(expression));
        }

        return builder.ToString();
    }

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

    private string? Print(LiteralExpression expression) => expression.Literal switch
    {
        double @double => @double.ToString(CultureInfo.InvariantCulture),
        null => "nil",
        _ => expression.Literal.ToString()
    };
}