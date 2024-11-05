using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Syntax.Statements;

public class VarStatement(Token name, IExpression? initializer) : IStatement
{
    public IExpression? Initializer { get; } = initializer;
    public Token Name { get; } = name;
}