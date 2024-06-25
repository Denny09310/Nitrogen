using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Syntax.Expressions;

internal class AssignmentExpression(Token name, IExpression value) : IExpression
{
    public Token Name { get; } = name;
    public IExpression Value { get; } = value;
}