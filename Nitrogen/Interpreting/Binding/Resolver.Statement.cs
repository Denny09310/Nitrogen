using Nitrogen.Syntax.Abstractions;
using Nitrogen.Syntax.Statements;

namespace Nitrogen.Interpreting.Binding;

internal partial class Resolver
{
    private void Resolve(IStatement statement)
    {
        switch (statement)
        {
            case PrintStatement print: Resolve(print); break;
            case VariableDeclarationStatement variable: Resolve(variable); break;
            default: break;
        }
    }

    private void Resolve(PrintStatement statement)
    {
        Resolve(statement.Expression);
    }

    private void Resolve(VariableDeclarationStatement statement)
    {
        Define(statement.Name);
        if (statement.Initializer is not null)
        {
            Resolve(statement.Initializer);
        }
        Declare(statement.Name);
    }
}