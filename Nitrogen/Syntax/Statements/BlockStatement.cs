using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Syntax.Statements;

internal class BlockStatement(List<IStatement> statements) : IStatement
{
    public List<IStatement> Statements { get; } = statements;
}