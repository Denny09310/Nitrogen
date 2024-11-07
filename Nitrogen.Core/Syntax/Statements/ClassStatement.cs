using Nitrogen.Core;
using Nitrogen.Core.Syntax.Expressions;
using Nitrogen.Core.Syntax.Statements.Abstractions;

namespace Nitrogen.Core.Syntax.Statements;

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