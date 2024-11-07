using Nitrogen.Core;
using Nitrogen.Core.Syntax.Expressions.Abstractions;
using Nitrogen.Core.Syntax.Statements.Abstractions;

namespace Nitrogen.Core.Syntax.Statements;

public class FunctionStatement(Token name, List<IExpression> arguments, IStatement body) : IStatement
{
    public List<IExpression> Arguments { get; } = arguments;
    public IStatement Body { get; } = body;
    public Token Name { get; } = name;
}

public enum FunctionType
{
    None,
    Function,
    Method,
    Constructor,
}