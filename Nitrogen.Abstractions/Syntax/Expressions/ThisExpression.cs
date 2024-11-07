using Nitrogen.Abstractions;
using Nitrogen.Abstractions.Syntax.Expressions.Abstractions;

namespace Nitrogen.Abstractions.Syntax.Expressions;

public class ThisExpression(Token keyword) : IExpression
{
    public Token Keyword { get; } = keyword;
}