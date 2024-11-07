using Nitrogen.Core.Syntax.Expressions.Abstractions;
using Nitrogen.Core.Syntax.Statements.Abstractions;

namespace Nitrogen.Core.Syntax.Statements;

public class ImportStatement(IEnumerable<IExpression> imports, IExpression source) : IStatement
{
    public IEnumerable<IExpression> Imports { get; } = imports;
    public IExpression Source { get; } = source;
}
