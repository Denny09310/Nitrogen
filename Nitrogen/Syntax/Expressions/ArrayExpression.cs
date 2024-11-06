using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Syntax.Expressions;

public class ArrayExpression(IEnumerable<IExpression> items) : IExpression
{
    public IEnumerable<IExpression> Items { get; } = items;
}