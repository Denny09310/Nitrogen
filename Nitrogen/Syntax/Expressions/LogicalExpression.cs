using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Syntax.Expressions;

public class LogicalExpression(IExpression left, Token @operator, IExpression right) : IExpression
{
    public IExpression Left { get; } = left;
    public Token Operator { get; } = @operator;
    public IExpression Right { get; } = right;
}