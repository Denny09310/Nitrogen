using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Syntax.Expressions;

public class ReturnExpression(Token keyword, IExpression? value) : IExpression
{
    public Token Keyword { get; } = keyword;
    public IExpression? Value { get; } = value;
}