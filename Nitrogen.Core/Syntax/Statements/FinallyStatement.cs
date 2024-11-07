using Nitrogen.Core.Syntax.Statements.Abstractions;

namespace Nitrogen.Core.Syntax.Statements;

public class FinallyStatement(Token keyword, IStatement body) : IStatement
{
    public Token Keyword { get; } = keyword;
    public IStatement Body { get; } = body;
}