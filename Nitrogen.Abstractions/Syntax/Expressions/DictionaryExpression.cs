using Nitrogen.Abstractions.Syntax.Expressions.Abstractions;

namespace Nitrogen.Abstractions.Syntax.Expressions;

public class DictionaryExpression(IDictionary<Token, object> items) : IExpression
{
    public IDictionary<Token, object> Items { get; } = items;
}
