using Nitrogen.Syntax;

namespace Nitrogen.Exceptions;

public class ParseException(Token token, string message) : Exception($"Parse error at line {token.Span.Start.Line}, column {token.Span.Start.Column}: {message} (near '{token.Lexeme}')")
{
    public Token Token { get; } = token;
}