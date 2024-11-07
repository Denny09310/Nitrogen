using Nitrogen.Abstractions;
using Nitrogen.Abstractions.Syntax.Expressions.Abstractions;

namespace Nitrogen.Abstractions.Syntax.Expressions;

public class GetterExpression(Token name, IExpression expression) : IExpression
{
    public IExpression Expression { get; } = expression;
    public Token Name { get; } = name;
}