using Nitrogen.Core;
using Nitrogen.Core.Syntax.Expressions.Abstractions;

namespace Nitrogen.Core.Syntax.Expressions;

public class SetterExpression(Token name, IExpression @object, IExpression value) : IExpression
{
    public Token Name { get; } = name;
    public IExpression Object { get; } = @object;
    public IExpression Value { get; } = value;
}