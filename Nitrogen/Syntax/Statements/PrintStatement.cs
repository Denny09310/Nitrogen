using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Syntax.Statements;

public class PrintStatement(IExpression expression) : IStatement
{
    public IExpression Expression { get; } = expression;
}