using Nitrogen.Abstractions.Syntax.Expressions.Abstractions;
using Nitrogen.Abstractions.Syntax.Statements.Abstractions;

namespace Nitrogen.Abstractions.Syntax.Statements;

public class ExpressionStatement(IExpression expression) : IStatement
{
    public IExpression Expression { get; } = expression;
}