using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Syntax.Expressions;

internal class ThisExpression(Token keyword) : IExpression
{
    public Token Keyword { get; } = keyword;
}