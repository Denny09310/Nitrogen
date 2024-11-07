using Nitrogen.Abstractions;
using Nitrogen.Abstractions.Syntax.Expressions;
using Nitrogen.Abstractions.Syntax.Statements.Abstractions;

namespace Nitrogen.Abstractions.Syntax.Statements;

public class ClassStatement(Token name, IdentifierExpression? superclass, IList<FunctionStatement> methods) : IStatement
{
    public IList<FunctionStatement> Methods { get; } = methods;
    public Token Name { get; } = name;
    public IdentifierExpression? Superclass { get; } = superclass;
}

public enum ClassType
{
    None,
    Class,
    Subclass
}