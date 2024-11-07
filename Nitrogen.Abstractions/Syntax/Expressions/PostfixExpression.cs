using Nitrogen.Abstractions;
using Nitrogen.Abstractions.Syntax.Expressions.Abstractions;

namespace Nitrogen.Abstractions.Syntax.Expressions;

public class PostfixExpression(Token @operator, IExpression identifier) : IExpression
{
    public IExpression Identifier { get; } = identifier;
    public Token Operator { get; } = @operator;
}