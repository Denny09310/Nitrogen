using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Syntax.Expressions;

internal class IdentifierExpression(Token name) : IExpression
{
    public Token Name { get; } = name;
}