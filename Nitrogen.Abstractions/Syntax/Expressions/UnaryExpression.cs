using Nitrogen.Abstractions;
using Nitrogen.Abstractions.Syntax.Expressions.Abstractions;

namespace Nitrogen.Abstractions.Syntax.Expressions;

public class UnaryExpression(Token @operator, IExpression expression) : IExpression
{
    public IExpression Expression { get; } = expression;
    public Token Operator { get; } = @operator;
}