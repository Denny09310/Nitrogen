using Nitrogen.Syntax.Abstractions;
using Nitrogen.Syntax.Expressions;

namespace Nitrogen.Syntax.Statements;

internal class ClassStatement(Token name, IdentifierExpression? superclass, IList<FunctionStatement> methods) : IStatement
{
    public IList<FunctionStatement> Methods { get; } = methods;
    public Token Name { get; } = name;
    public IdentifierExpression? Superclass { get; } = superclass;
}

internal enum ClassType
{
    None,
    Class,
    Subclass
}