using Nitrogen.Core;
using Nitrogen.Core.Syntax.Expressions.Abstractions;

namespace Nitrogen.Core.Syntax.Expressions;

public class PrefixExpression(Token @operator, IExpression identifier) : IExpression
{
    public IExpression Identifier { get; } = identifier;
    public Token Operator { get; } = @operator;
}