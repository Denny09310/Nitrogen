using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Syntax.Expressions;

internal class ReturnExpression(Token keyword, IExpression? value) : IExpression
{
    public Token Keyword { get; } = keyword;
    public IExpression? Value { get; } = value;
}