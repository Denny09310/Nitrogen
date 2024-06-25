using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Syntax.Statements;

internal class FunctionStatement(Token name, List<IExpression> arguments, IStatement body) : IStatement
{
    public List<IExpression> Arguments { get; } = arguments;
    public IStatement Body { get; } = body;
    public Token Name { get; } = name;
}