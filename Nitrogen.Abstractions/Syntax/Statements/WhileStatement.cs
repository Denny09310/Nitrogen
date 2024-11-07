using Nitrogen.Abstractions;
using Nitrogen.Abstractions.Syntax.Expressions.Abstractions;
using Nitrogen.Abstractions.Syntax.Statements.Abstractions;

namespace Nitrogen.Abstractions.Syntax.Statements;

public class WhileStatement(Token keyword, IExpression condition, IStatement body) : IStatement
{
    public IStatement Body { get; } = body;
    public IExpression Condition { get; } = condition;
    public Token Keyword { get; } = keyword;
}

public enum LoopType
{
    None,
    While,
    For
}