using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Syntax.Expressions;

public class IdentifierExpression(Token name) : IExpression
{
    public Token Name { get; } = name;
}