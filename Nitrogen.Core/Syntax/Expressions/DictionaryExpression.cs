using Nitrogen.Core.Syntax.Expressions.Abstractions;

namespace Nitrogen.Core.Syntax.Expressions;

public class DictionaryExpression(IDictionary<Token, IExpression> items) : IExpression
{
    public IDictionary<Token, IExpression> Value { get; } = items;
}
