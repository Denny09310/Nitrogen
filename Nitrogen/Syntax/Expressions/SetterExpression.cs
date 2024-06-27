using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Syntax.Expressions;

internal class SetterExpression(Token name, IExpression @object, IExpression value) : IExpression
{
    public Token Name { get; } = name;
    public IExpression Object { get; } = @object;
    public IExpression Value { get; } = value;
}