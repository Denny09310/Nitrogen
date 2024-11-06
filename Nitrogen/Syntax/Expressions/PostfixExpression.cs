using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Syntax.Expressions;

public class PostfixExpression(Token @operator, IExpression identifier) : IExpression
{
    public IExpression Identifier { get; } = identifier;
    public Token Operator { get; } = @operator;
}