using Nitrogen.Abstractions;
using Nitrogen.Abstractions.Syntax.Expressions.Abstractions;
using Nitrogen.Abstractions.Syntax.Statements.Abstractions;

namespace Nitrogen.Abstractions.Syntax.Statements;

public class ForStatement(Token keyword, IStatement? initialization, IExpression condition, IStatement body, IExpression? increment) : IStatement
{
    public IStatement Body { get; } = body;
    public IExpression Condition { get; } = condition;
    public IExpression? Increment { get; } = increment;
    public IStatement? Initialization { get; } = initialization;
    public Token Keyword { get; } = keyword;
}