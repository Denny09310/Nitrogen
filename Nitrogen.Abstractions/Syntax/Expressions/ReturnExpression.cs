using Nitrogen.Abstractions;
using Nitrogen.Abstractions.Syntax.Expressions.Abstractions;

namespace Nitrogen.Abstractions.Syntax.Expressions;

public class ReturnExpression(Token keyword, IExpression? value) : IExpression
{
    public Token Keyword { get; } = keyword;
    public IExpression? Value { get; } = value;
}