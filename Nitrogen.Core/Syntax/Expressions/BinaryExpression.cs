using Nitrogen.Core;
using Nitrogen.Core.Syntax.Expressions.Abstractions;

namespace Nitrogen.Core.Syntax.Expressions;

public class BinaryExpression(IExpression left, Token @operator, IExpression right) : IExpression
{
    public IExpression Left { get; } = left;
    public Token Operator { get; } = @operator;
    public IExpression Right { get; } = right;
}