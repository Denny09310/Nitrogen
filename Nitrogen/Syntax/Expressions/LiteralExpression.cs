using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Syntax.Expressions;

internal class LiteralExpression(object? literal) : IExpression
{
    public object? Literal { get; } = literal;
}