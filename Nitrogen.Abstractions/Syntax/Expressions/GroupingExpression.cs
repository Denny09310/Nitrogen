using Nitrogen.Abstractions;
using Nitrogen.Abstractions.Syntax.Expressions.Abstractions;

namespace Nitrogen.Abstractions.Syntax.Expressions;

public class GroupingExpression(Token paren, IExpression expression) : IExpression
{
    public IExpression Expression { get; } = expression;
    public Token Paren { get; } = paren;
}