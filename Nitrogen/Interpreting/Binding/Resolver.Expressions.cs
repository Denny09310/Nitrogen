using Nitrogen.Syntax.Abstractions;
using Nitrogen.Syntax.Expressions;

namespace Nitrogen.Interpreting.Binding;

internal partial class Resolver
{
    private void Resolve(IExpression expression)
    {
        switch (expression)
        {
            case AssignmentExpression assignment: Resolve(assignment); break;
            case IdentifierExpression identifier: Resolve(identifier); break;
            case LiteralExpression: break;
            default: break;
        }
    }

    private void Resolve(AssignmentExpression statement)
    {
    }

    private void Resolve(IdentifierExpression statement)
    {
        ResolveLocal(statement.Name);
    }
}