using Nitrogen.Core;
using Nitrogen.Core.Syntax.Expressions.Abstractions;

namespace Nitrogen.Core.Syntax.Expressions;

public class ReturnExpression(Token keyword, IExpression? value) : IExpression
{
    public Token Keyword { get; } = keyword;
    public IExpression? Value { get; } = value;
}