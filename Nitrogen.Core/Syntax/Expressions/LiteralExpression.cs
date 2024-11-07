using Nitrogen.Core.Syntax.Expressions.Abstractions;

namespace Nitrogen.Core.Syntax.Expressions;

public class LiteralExpression(object? literal) : IExpression
{
    public object? Literal { get; } = literal;
}