using Nitrogen.Core;
using Nitrogen.Core.Syntax.Expressions.Abstractions;

namespace Nitrogen.Core.Syntax.Expressions;

public class IdentifierExpression(Token name) : IExpression
{
    public Token Name { get; } = name;
}