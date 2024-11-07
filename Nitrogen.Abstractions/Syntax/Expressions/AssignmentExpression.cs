using Nitrogen.Abstractions;
using Nitrogen.Abstractions.Syntax.Expressions.Abstractions;

namespace Nitrogen.Abstractions.Syntax.Expressions;

public class AssignmentExpression(Token name, IExpression value) : IExpression
{
    public Token Name { get; } = name;
    public IExpression Value { get; } = value;
}