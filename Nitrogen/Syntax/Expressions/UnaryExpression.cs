using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Syntax.Expressions;

internal class UnaryExpression(Token @operator, IExpression expression) : IExpression
{
    public IExpression Expression { get; } = expression;
    public Token Operator { get; } = @operator;
}