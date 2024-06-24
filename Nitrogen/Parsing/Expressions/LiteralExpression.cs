using Nitrogen.Parsing.Expressions.Abstractions;

namespace Nitrogen.Parsing.Expressions;

internal class LiteralExpression(object? literal) : IExpression
{
    public object? Literal { get; } = literal;
}