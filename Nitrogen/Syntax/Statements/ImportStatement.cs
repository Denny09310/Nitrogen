using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Syntax.Statements;

public class ImportStatement(IEnumerable<IExpression> imports, IExpression source) : IStatement
{
    public IEnumerable<IExpression> Imports { get; } = imports;
    public IExpression Source { get; } = source;
}
