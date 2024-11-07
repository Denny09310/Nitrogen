using Nitrogen.Core;
using Nitrogen.Core.Syntax.Expressions.Abstractions;

namespace Nitrogen.Core.Syntax.Expressions;

public class GetterExpression(Token name, IExpression expression) : IExpression
{
    public IExpression Expression { get; } = expression;
    public Token Name { get; } = name;
}