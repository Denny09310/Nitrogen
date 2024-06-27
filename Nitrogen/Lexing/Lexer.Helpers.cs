using Nitrogen.Syntax;

namespace Nitrogen.Lexing;

internal partial class Lexer
{
    private static readonly Dictionary<string, TokenKind> _keywords = new()
    {
        ["true"] = TokenKind.True,
        ["false"] = TokenKind.False,
        ["print"] = TokenKind.Print,
        ["and"] = TokenKind.And,
        ["or"] = TokenKind.Or,
        ["while"] = TokenKind.While,
        ["for"] = TokenKind.For,
        ["var"] = TokenKind.Var,
        ["break"] = TokenKind.Break,
        ["continue"] = TokenKind.Continue,
        ["return"] = TokenKind.Return,
        ["if"] = TokenKind.If,
        ["else"] = TokenKind.Else,
        ["function"] = TokenKind.Function,
        ["class"] = TokenKind.Class,
        ["this"] = TokenKind.This,
        ["super"] = TokenKind.Super,
    };

    private char Advance()
    {
        var current = Peek();

        _column++;
        _index++;

        return current;
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
            TokenKind.Number => double.Parse(lexeme),
            TokenKind.String => lexeme.Replace("\\\"", "\""),
            _ => null,
        };

        (var location, _location) = (_location, new SourceLocation(_line, _column));

        return new Token { Kind = kind, Lexeme = lexeme, Value = value, Span = location.AsSpan(_location) };
    }

    private bool IsLastCharacter() => source.CharAt(_index) == '\0';

    private bool Match(char expected)
    {
        if (Peek() == expected)
        {
            Consume();
            return true;
        }

        return false;
    }

    private char Peek(int count = 0) => source.CharAt(_index + count);
}