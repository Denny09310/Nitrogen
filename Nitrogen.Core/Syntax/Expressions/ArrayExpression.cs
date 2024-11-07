using Nitrogen.Core.Syntax.Expressions.Abstractions;

namespace Nitrogen.Core.Syntax.Expressions;

public class ArrayExpression(IEnumerable<IExpression> items) : IExpression
{
    public IEnumerable<IExpression> Items { get; } = items;
}