using Nitrogen.Abstractions.Syntax.Expressions.Abstractions;

namespace Nitrogen.Abstractions.Syntax.Expressions;

public class DictionaryExpression(IDictionary<Token, IExpression> items) : IExpression
{
    public IDictionary<Token, IExpression> Value { get; } = items;
}
