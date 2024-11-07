using Nitrogen.Core.Syntax.Expressions.Abstractions;
using Nitrogen.Core.Syntax.Statements.Abstractions;

namespace Nitrogen.Core.Interpreting;

public interface IInterpreter
{
    IEnvironment Environment { get; }
    Dictionary<IExpression, int> Locals { get; }

    object? Evaluate(IExpression expr);
    void Execute(List<IStatement> statements);
    void Execute(List<IStatement> statements, IEnvironment environment);
    void Execute(IStatement statement, IEnvironment environment);
}
