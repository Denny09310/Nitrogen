using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Syntax.Statements;

internal class ExpressionStatement(IExpression expression) : IStatement
{
    public IExpression Expression { get; } = expression;
}