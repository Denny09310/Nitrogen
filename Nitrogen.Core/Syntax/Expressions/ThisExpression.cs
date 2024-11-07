using Nitrogen.Core;
using Nitrogen.Core.Syntax.Expressions.Abstractions;

namespace Nitrogen.Core.Syntax.Expressions;

public class ThisExpression(Token keyword) : IExpression
{
    public Token Keyword { get; } = keyword;
}