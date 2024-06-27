using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Syntax.Statements;

internal class ClassStatement(Token name, IList<FunctionStatement> methods) : IStatement
{
    public IList<FunctionStatement> Methods { get; } = methods;
    public Token Name { get; } = name;
}

internal enum ClassType
{
    None,
    Class,
    Subclass
}