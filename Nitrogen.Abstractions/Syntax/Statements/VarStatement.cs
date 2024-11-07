using Nitrogen.Abstractions;
using Nitrogen.Abstractions.Syntax.Expressions.Abstractions;
using Nitrogen.Abstractions.Syntax.Statements.Abstractions;

namespace Nitrogen.Abstractions.Syntax.Statements;

public class VarStatement(Token name, IExpression? initializer) : IStatement
{
    public IExpression? Initializer { get; } = initializer;
    public Token Name { get; } = name;
}