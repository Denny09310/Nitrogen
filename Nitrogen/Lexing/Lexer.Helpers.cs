using Nitrogen.Abstractions;
using System.Globalization;

namespace Nitrogen.Lexing;

public partial class Lexer
{
    private static readonly Dictionary<string, TokenKind> _keywords = new()
    {
        ["true"] = TokenKind.True,
        ["false"] = TokenKind.False,
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
        ["extends"] = TokenKind.Extends,
        ["import"] = TokenKind.Import,
        ["from"] = TokenKind.From,
    };

    private static object? GetValue(TokenKind kind, string lexeme)
    {
        return kind switch
        {
            TokenKind.Number => double.Parse(lexeme, CultureInfo.InvariantCulture),
            TokenKind.String => lexeme.Replace("\\\"", "\""),
            _ => null
        };
    }

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
        var lexeme = ExtractLexeme();
        var value = GetValue(kind, lexeme);

        (var location, _location) = (_location, new SourceLocation(_line, _column));

        return new Token { Kind = kind, Lexeme = lexeme, Value = value, Span = location.AsSpan(_location) };
    }

    private string ExtractLexeme()
    {
        var lexeme = _buffer.ToString();
        _buffer.Clear();
        return lexeme;
    }

    private void HandleEscapeSequence()
    {
        switch (Peek())
        {
            case 'n': _buffer.Append('\n'); break;
            case 't': _buffer.Append('\t'); break;
            case '\\': _buffer.Append('\\'); break;
            case '\"': _buffer.Append('\"'); break;
        }

        Advance();
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