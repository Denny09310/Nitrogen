using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Syntax.Expressions;

internal class CallExpression(Token name, List<IExpression> parameters) : IExpression
{
    public Token Name { get; } = name;
    public List<IExpression> Parameters { get; } = parameters;
}