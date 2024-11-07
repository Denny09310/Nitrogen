using Nitrogen.Abstractions;
using Nitrogen.Abstractions.Syntax.Expressions.Abstractions;

namespace Nitrogen.Abstractions.Syntax.Expressions;

public class BinaryExpression(IExpression left, Token @operator, IExpression right) : IExpression
{
    public IExpression Left { get; } = left;
    public Token Operator { get; } = @operator;
    public IExpression Right { get; } = right;
}