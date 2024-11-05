using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Syntax.Expressions;

public class LiteralExpression(object? literal) : IExpression
{
    public object? Literal { get; } = literal;
}