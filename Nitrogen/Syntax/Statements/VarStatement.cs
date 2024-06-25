using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Syntax.Statements;

internal class VarStatement(Token name, IExpression? initializer) : IStatement
{
    public IExpression? Initializer { get; } = initializer;
    public Token Name { get; } = name;
}