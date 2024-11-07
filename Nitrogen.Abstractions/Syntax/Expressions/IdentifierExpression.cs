using Nitrogen.Abstractions;
using Nitrogen.Abstractions.Syntax.Expressions.Abstractions;

namespace Nitrogen.Abstractions.Syntax.Expressions;

public class IdentifierExpression(Token name) : IExpression
{
    public Token Name { get; } = name;
}