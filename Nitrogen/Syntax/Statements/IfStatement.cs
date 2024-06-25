using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Syntax.Statements;

internal class IfStatement(Token keyword, IExpression condition, IStatement then, IStatement? @else) : IStatement
{
    public IExpression Condition { get; } = condition;
    public IStatement? Else { get; } = @else;
    public Token Keyword { get; } = keyword;
    public IStatement Then { get; } = then;
}