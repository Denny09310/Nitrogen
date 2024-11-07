using Nitrogen.Core.Syntax.Expressions.Abstractions;
using Nitrogen.Core.Syntax.Statements.Abstractions;

namespace Nitrogen.Core.Syntax.Statements;

public class ExpressionStatement(IExpression expression) : IStatement
{
    public IExpression Expression { get; } = expression;
}