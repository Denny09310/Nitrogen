using Nitrogen.Abstractions.Syntax.Expressions.Abstractions;
using Nitrogen.Abstractions.Syntax.Statements.Abstractions;

namespace Nitrogen.Abstractions.Interpreting;

public interface IInterpreter
{
    IEnvironment Environment { get; }
    Dictionary<IExpression, int> Locals { get; }

    object? Evaluate(IExpression expr);
    void Execute(List<IStatement> statements);
    void Execute(List<IStatement> statements, IEnvironment environment);
}
