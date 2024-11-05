using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Syntax.Statements;

public class ExpressionStatement(IExpression expression) : IStatement
{
    public IExpression Expression { get; } = expression;
}