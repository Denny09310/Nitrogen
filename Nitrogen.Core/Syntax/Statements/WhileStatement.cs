using Nitrogen.Core;
using Nitrogen.Core.Syntax.Expressions.Abstractions;
using Nitrogen.Core.Syntax.Statements.Abstractions;

namespace Nitrogen.Core.Syntax.Statements;

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