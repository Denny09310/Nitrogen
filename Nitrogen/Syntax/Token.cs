namespace Nitrogen.Syntax;

public readonly record struct Token
{
    public TokenKind Kind { get; init; }
    public SourceSpan Span { get; init; }
    public string Lexeme { get; init; }
    public object? Value { get; init; }
}

public enum TokenKind
{
    #region Operators

    Plus,
    Minus,
    Star,
    Slash,
    Percentage,
    Ampersand,
    Pipe,
    BackSlash,
    Equal,
    Bang,
    Less,
    Greater,

    #endregion Operators

    #region Compound operators

    PlusPlus,
    MinusMinus,
    PipePipe,
    AmpersandAmpersand,
    EqualEqual,
    BangEqual,
    LessEqual,
    GreaterEqual,

    #endregion Compound operators

    #region Delimiters

    LeftParenthesis,
    RightParenthesis,
    LeftBracket,
    RightBracket,
    LeftBrace,
    RightBrace,
    Dot,
    Comma,
    Semicolon,
    Colon,
    Question,
    DoubleQuote,
    SingleQuote,

    #endregion Delimiters

    #region Keywords

    True,
    False,
    While,
    Print,
    And,
    Or,
    For,
    Var,
    Break,
    Continue,
    Return,
    If,
    Else,
    Function,
    Class,
    This,
    Super,

    #endregion Keywords

    #region Identifiers and Literals

    Identifier,
    String,
    Number,

    #endregion Identifiers and Literals

    #region Whitespace and control characters

    WhiteSpace,
    NewLine,

    #endregion Whitespace and control characters

    #region Miscellaneous

    EOF,
    Error,
    LineComment,
    BlockComment,

    #endregion Miscellaneous
}