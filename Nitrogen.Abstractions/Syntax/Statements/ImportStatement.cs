using Nitrogen.Abstractions.Syntax.Expressions.Abstractions;
using Nitrogen.Abstractions.Syntax.Statements.Abstractions;

namespace Nitrogen.Abstractions.Syntax.Statements;

public class ImportStatement(IEnumerable<IExpression> imports, IExpression source) : IStatement
{
    public IEnumerable<IExpression> Imports { get; } = imports;
    public IExpression Source { get; } = source;
}
