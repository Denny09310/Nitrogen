using Nitrogen.Syntax;

namespace Nitrogen;

internal partial class Lexer
{
    private static readonly Dictionary<string, TokenKind> _keywords = new()
    {
        ["true"] = TokenKind.True,
        ["false"] = TokenKind.False,
    };

    private char Advance()
    {
        var current = Peek();

        _column++;
        _index++;

        return current;
    }

    private void AdvanceNewLine()
    {
        _line++;
        _column = 1;
    }

    private char Consume()
    {
        var current = Advance();
        _buffer.Append(current);
        return current;
    }

    private Token CreateToken(TokenKind kind)
    {
        var lexeme = _buffer.ToString();
        _buffer.Clear();

        object? value = kind switch
        {
            TokenKind.Integer => int.Parse(lexeme),
            TokenKind.Float => float.Parse(lexeme),
            _ => null,
        };

        (var location, _location) = (_location, new SourceLocation(_line, _column));

        return new Token { Kind = kind, Lexeme = lexeme, Value = value, Span = location.AsSpan(_location) };
    }

    private bool IsLastCharacter() => source.CharAt(_index) == '\0';

    private char Peek(int count = 0) => source.CharAt(_index + count);
}