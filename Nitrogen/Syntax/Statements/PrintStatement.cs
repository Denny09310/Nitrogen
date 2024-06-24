using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Syntax.Statements;

internal class PrintStatement(IExpression expression) : IStatement
{
    public IExpression Expression { get; } = expression;
}