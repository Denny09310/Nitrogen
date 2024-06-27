using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Syntax.Expressions;

internal class GetterExpression(Token name, IExpression expression) : IExpression
{
    public IExpression Expression { get; } = expression;
    public Token Name { get; } = name;
}