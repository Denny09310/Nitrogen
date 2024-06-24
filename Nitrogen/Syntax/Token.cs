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
    Number,
    Plus,
    EOF,
    WhiteSpace,
    NewLine,
    Identifier,
    True,
    False,
    Minus,
    Star,
    Slash,
    LeftParenthesis,
    RightParenthesis,
    Exclamation,
    Equal,
    Less,
    Greater,
    Dot,
    Comma,
    Semicolon,
    Colon,
    Question,
    Quote,
    SingleQuote,
    SquareLeft,
    SquareRight,
    CurlyLeft,
    CurlyRight,
    Percentage,
    Ampersand,
    BackSlash,
    Pipe,
    Print,
    PipePipe,
    AmpersandAmpersand,
    And,
    Or,
}