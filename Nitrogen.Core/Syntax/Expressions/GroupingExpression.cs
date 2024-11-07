using Nitrogen.Core;
using Nitrogen.Core.Syntax.Expressions.Abstractions;

namespace Nitrogen.Core.Syntax.Expressions;

public class GroupingExpression(Token paren, IExpression expression) : IExpression
{
    public IExpression Expression { get; } = expression;
    public Token Paren { get; } = paren;
}