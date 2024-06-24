using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Syntax.Expressions;

internal class GroupingExpression(Token paren, IExpression expression) : IExpression
{
    public IExpression Expression { get; } = expression;
    public Token Paren { get; } = paren;
}