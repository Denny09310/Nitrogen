using Nitrogen.Core;
using Nitrogen.Core.Syntax.Expressions.Abstractions;

namespace Nitrogen.Core.Syntax.Expressions;

public class CallExpression(Token paren, IExpression target, List<IExpression> parameters) : IExpression
{
    public List<IExpression> Parameters { get; } = parameters;
    public Token Paren { get; } = paren;
    public IExpression Target { get; } = target;
}