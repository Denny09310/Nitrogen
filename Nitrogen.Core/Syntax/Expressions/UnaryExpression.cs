using Nitrogen.Core;
using Nitrogen.Core.Syntax.Expressions.Abstractions;

namespace Nitrogen.Core.Syntax.Expressions;

public class UnaryExpression(Token @operator, IExpression expression) : IExpression
{
    public IExpression Expression { get; } = expression;
    public Token Operator { get; } = @operator;
}