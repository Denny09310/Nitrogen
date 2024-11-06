using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Syntax.Expressions;

public class IndexExpression(Token bracket, IExpression array, IExpression index) : IExpression
{
    public IExpression Array { get; } = array;
    public Token Bracket { get; } = bracket;
    public IExpression Index { get; } = index;
}