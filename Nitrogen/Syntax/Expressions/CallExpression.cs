using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Syntax.Expressions;

internal class CallExpression(Token paren, IExpression target, List<IExpression> parameters) : IExpression
{
    public List<IExpression> Parameters { get; } = parameters;
    public Token Paren { get; } = paren;
    public IExpression Target { get; } = target;
}