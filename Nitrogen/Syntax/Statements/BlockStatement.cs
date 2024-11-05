using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Syntax.Statements;

public class BlockStatement(List<IStatement> statements) : IStatement
{
    public List<IStatement> Statements { get; } = statements;
}