using Nitrogen.Parsing.Expressions.Abstractions;
using Nitrogen.Syntax;

namespace Nitrogen.Parsing.Expressions;

internal class BinaryExpression(IExpression left, Token @operator, IExpression right) : IExpression
{
    public IExpression Left { get; } = left;
    public Token Operator { get; } = @operator;
    public IExpression Right { get; } = right;
}