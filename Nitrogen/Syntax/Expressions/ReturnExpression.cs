using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Syntax.Expressions;

internal class ReturnExpression(IExpression? value) : IExpression
{
    public IExpression? Value { get; } = value;
}