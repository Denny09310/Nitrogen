using Nitrogen.Syntax;
using System.Diagnostics;
using System.Text;

namespace Nitrogen.Lexing;

internal partial class Lexer(SourceText source)
{
    private readonly StringBuilder _buffer = new();
    private readonly List<Token> _tokens = [];

    private int _line, _index, _column = 1;
    private SourceLocation _location;

    public static Lexer FromSource(string source) => new(new SourceText(source));

    public List<Token> Tokenize()
    {
        CreateToken(TokenKind.EOF);

        while (!IsLastCharacter())
        {
            if (Lex() is not Token token) continue;
            _tokens.Add(token);
        }

        _tokens.Add(CreateToken(TokenKind.EOF));
        return _tokens;
    }

    private Token? Lex()
    {
        char current = Peek();

        if (current is '\n')
        {
            LexNewLine();
            return null;
        }

        if (char.IsWhiteSpace(current))
        {
            LexWhiteSpace();
            return null;
        }

        if (char.IsDigit(current))
        {
            return LexNumber();
        }

        if (char.IsLetter(current) || current == '_')
        {
            return LexIdentifier();
        }

        if (current == '"')
        {
            return LexString();
        }

        return LexPunctuation();
    }

    private Token LexIdentifier()
    {
        while ((char.IsLetterOrDigit(Peek()) || Peek() == '_') && !IsLastCharacter())
        {
            Consume();
        }

        if (_keywords.TryGetValue(_buffer.ToString(), out var keyword))
        {
            return CreateToken(kind: keyword);
        }

        return CreateToken(TokenKind.Identifier);
    }

    private void LexNewLine()
    {
        Consume();

        _line++;
        _column = 1;

        CreateToken(TokenKind.NewLine);
    }

    private Token LexNumber()
    {
        while ((char.IsDigit(Peek()) || Peek() is '.') && !IsLastCharacter())
        {
            Consume();
        }

        return CreateToken(TokenKind.Number);
    }

    private Token LexPunctuation()
    {
        var current = Consume();
        switch (current)
        {
            case '*': return CreateToken(TokenKind.Star);
            case '/': return CreateToken(TokenKind.Slash);
            case '\\': return CreateToken(TokenKind.BackSlash);
            case '(': return CreateToken(TokenKind.LeftParenthesis);
            case ')': return CreateToken(TokenKind.RightParenthesis);
            case '?': return CreateToken(TokenKind.Question);
            case '.': return CreateToken(TokenKind.Dot);
            case ',': return CreateToken(TokenKind.Comma);
            case ';': return CreateToken(TokenKind.Semicolon);
            case ':': return CreateToken(TokenKind.Colon);
            case '"': return CreateToken(TokenKind.DoubleQuote);
            case '\'': return CreateToken(TokenKind.SingleQuote);
            case '[': return CreateToken(TokenKind.SquareLeft);
            case ']': return CreateToken(TokenKind.SquareRight);
            case '{': return CreateToken(TokenKind.CurlyLeft);
            case '}': return CreateToken(TokenKind.CurlyRight);
            case '%': return CreateToken(TokenKind.Percentage);

            case '-':
                if (Match('-')) return CreateToken(TokenKind.MinusMinus);
                return CreateToken(TokenKind.Minus);

            case '+':
                if (Match('+')) return CreateToken(TokenKind.PlusPlus);
                return CreateToken(TokenKind.Plus);

            case '<':
                if (Match('=')) return CreateToken(TokenKind.LessEqual);
                return CreateToken(TokenKind.Less);

            case '>':
                if (Match('=')) return CreateToken(TokenKind.GreaterEqual);
                return CreateToken(TokenKind.Greater);

            case '!':
                if (Match('=')) return CreateToken(TokenKind.BangEqual);
                return CreateToken(TokenKind.Bang);

            case '=':
                if (Match('=')) return CreateToken(TokenKind.EqualEqual);
                return CreateToken(TokenKind.Equal);

            case '&':
                if (Match('&')) return CreateToken(TokenKind.PipePipe);
                return CreateToken(TokenKind.Ampersand);

            case '|':
                if (Match('|')) return CreateToken(TokenKind.PipePipe);
                return CreateToken(TokenKind.Pipe);

            default:
                throw new UnreachableException($"The character '{current}' can't be tokenized");
        }
    }

    private Token? LexString()
    {
        Advance();

        while (Peek() != '"' && !IsLastCharacter())
        {
            if (Peek() == '\\') Consume();
            Consume();
        }

        Advance();

        return CreateToken(TokenKind.String);
    }

    private void LexWhiteSpace()
    {
        while (char.IsWhiteSpace(Peek()) && !IsLastCharacter())
        {
            Consume();
        }

        CreateToken(TokenKind.WhiteSpace);
    }
}