using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Syntax.Expressions;

public class ThisExpression(Token keyword) : IExpression
{
    public Token Keyword { get; } = keyword;
}