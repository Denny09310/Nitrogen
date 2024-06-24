using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Syntax.Expressions;

internal class AssignmentExpression(Token target, IExpression value) : IExpression
{
    public Token Target { get; } = target;
    public IExpression Value { get; } = value;
}