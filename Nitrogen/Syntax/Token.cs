namespace Nitrogen.Syntax;

internal readonly record struct Token
{
    public TokenKind Kind { get; init; }
    public SourceSpan Span { get; init; }
    public string Lexeme { get; init; }
    public object? Value { get; init; }
}

internal enum TokenKind
{
    Integer,
    Float,
    Plus,
    EOF,
    WhiteSpace,
    NewLine,
    Identifier,
    True,
    False,
    Minus,
    Star,
    Slash
}