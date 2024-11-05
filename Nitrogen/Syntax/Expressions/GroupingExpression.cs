using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Syntax.Expressions;

public class GroupingExpression(Token paren, IExpression expression) : IExpression
{
    public IExpression Expression { get; } = expression;
    public Token Paren { get; } = paren;
}