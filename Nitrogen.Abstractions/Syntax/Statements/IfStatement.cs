using Nitrogen.Abstractions;
using Nitrogen.Abstractions.Syntax.Expressions.Abstractions;
using Nitrogen.Abstractions.Syntax.Statements.Abstractions;

namespace Nitrogen.Abstractions.Syntax.Statements;

public class IfStatement(Token keyword, IExpression condition, IStatement then, IStatement? @else) : IStatement
{
    public IExpression Condition { get; } = condition;
    public IStatement? Else { get; } = @else;
    public Token Keyword { get; } = keyword;
    public IStatement Then { get; } = then;
}