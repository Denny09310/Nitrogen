using Nitrogen.Core.Syntax.Statements.Abstractions;

namespace Nitrogen.Core.Syntax.Statements;

public class BlockStatement(List<IStatement> statements) : IStatement
{
    public List<IStatement> Statements { get; } = statements;
}