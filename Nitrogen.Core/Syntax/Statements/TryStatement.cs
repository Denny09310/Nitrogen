using Nitrogen.Core.Syntax.Statements.Abstractions;

namespace Nitrogen.Core.Syntax.Statements;

public class TryStatement(Token keyword, IStatement @try, CatchStatement? @catch, FinallyStatement? @finally) : IStatement
{
    public IStatement Body { get; } = @try;
    public CatchStatement? Catch { get; } = @catch;
    public FinallyStatement? Finally { get; } = @finally;
    public Token Keyword { get; } = keyword;
}