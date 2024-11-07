using Nitrogen.Core;
using Nitrogen.Core.Syntax.Expressions.Abstractions;

namespace Nitrogen.Core.Syntax.Expressions;

public class IndexExpression(Token bracket, IExpression array, IExpression index) : IExpression
{
    public IExpression Array { get; } = array;
    public Token Bracket { get; } = bracket;
    public IExpression Index { get; } = index;
}