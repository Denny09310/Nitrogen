using Nitrogen.Core;
using Nitrogen.Core.Syntax.Expressions.Abstractions;
using Nitrogen.Core.Syntax.Statements.Abstractions;

namespace Nitrogen.Core.Syntax.Statements;

public class ForStatement(Token keyword, IStatement? initialization, IExpression condition, IStatement body, IExpression? increment) : IStatement
{
    public IStatement Body { get; } = body;
    public IExpression Condition { get; } = condition;
    public IExpression? Increment { get; } = increment;
    public IStatement? Initialization { get; } = initialization;
    public Token Keyword { get; } = keyword;
}