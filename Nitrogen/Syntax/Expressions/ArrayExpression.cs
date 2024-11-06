using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Syntax.Expressions;

public class ArrayExpression(Token paren, IEnumerable<IExpression> items) : IExpression
{
    public IEnumerable<IExpression> Items { get; } = items;
    public Token Paren { get; } = paren;
}