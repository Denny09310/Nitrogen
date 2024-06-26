using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Syntax.Statements;

internal class WhileStatement(Token keyword, IExpression condition, IStatement body) : IStatement
{
    public IStatement Body { get; } = body;
    public IExpression Condition { get; } = condition;
    public Token Keyword { get; } = keyword;
}

internal enum LoopType
{
    None,
    While,
    For,
}