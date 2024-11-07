using Nitrogen.Core;
using Nitrogen.Core.Syntax.Expressions.Abstractions;
using Nitrogen.Core.Syntax.Statements.Abstractions;

namespace Nitrogen.Core.Syntax.Statements;

public class VarStatement(Token name, IExpression? initializer) : IStatement
{
    public IExpression? Initializer { get; } = initializer;
    public Token Name { get; } = name;
}