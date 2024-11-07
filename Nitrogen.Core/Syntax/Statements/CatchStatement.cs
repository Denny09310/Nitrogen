using Nitrogen.Core.Exceptions;
using Nitrogen.Core.Syntax.Expressions;
using Nitrogen.Core.Syntax.Statements.Abstractions;

namespace Nitrogen.Core.Syntax.Statements;

public class CatchStatement(Token keyword, IdentifierExpression? identifier, BlockStatement @catch) : IStatement
{
    public BlockStatement Body { get; } = @catch;
    public IdentifierExpression? Identifier { get; } = identifier;
    public Token Keyword { get; } = keyword;

    public RuntimeException? Exception { get; set; }
}