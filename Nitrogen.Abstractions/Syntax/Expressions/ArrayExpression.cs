using Nitrogen.Abstractions.Syntax.Expressions.Abstractions;

namespace Nitrogen.Abstractions.Syntax.Expressions;

public class ArrayExpression(IEnumerable<IExpression> items) : IExpression
{
    public IEnumerable<IExpression> Items { get; } = items;
}