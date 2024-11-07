using Nitrogen.Core;
using Nitrogen.Core.Syntax.Expressions.Abstractions;

namespace Nitrogen.Core.Syntax.Expressions;

public class AssignmentExpression(Token name, IExpression value) : IExpression
{
    public Token Name { get; } = name;
    public IExpression Value { get; } = value;
}