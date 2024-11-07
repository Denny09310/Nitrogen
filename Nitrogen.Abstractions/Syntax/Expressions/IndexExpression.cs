using Nitrogen.Abstractions;
using Nitrogen.Abstractions.Syntax.Expressions.Abstractions;

namespace Nitrogen.Abstractions.Syntax.Expressions;

public class IndexExpression(Token bracket, IExpression array, IExpression index) : IExpression
{
    public IExpression Array { get; } = array;
    public Token Bracket { get; } = bracket;
    public IExpression Index { get; } = index;
}