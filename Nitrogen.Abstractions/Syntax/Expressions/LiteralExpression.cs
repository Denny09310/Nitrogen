using Nitrogen.Abstractions.Syntax.Expressions.Abstractions;

namespace Nitrogen.Abstractions.Syntax.Expressions;

public class LiteralExpression(object? literal) : IExpression
{
    public object? Literal { get; } = literal;
}