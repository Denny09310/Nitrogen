using Nitrogen.Exceptions;
using Nitrogen.Syntax;
using System.Text;

namespace Nitrogen.Lexing;

public partial class Lexer(SourceText source)
{
    private readonly StringBuilder _buffer = new();

    private readonly List<SyntaxException> _errors = [];
    private readonly List<Token> _tokens = [];

    private int _index, _line, _column;
    private SourceLocation _location;

    public static Lexer FromSource(string source) => new(new SourceText(source));

    public (List<Token>, List<SyntaxException>) Tokenize()
    {
        _line = 1;

        CreateToken(TokenKind.EOF);

        while (!IsLastCharacter())
        {
            try
            {
                if (Lex() is not Token token) continue;
                _tokens.Add(token);
            }
            catch (SyntaxException ex)
            {
                _errors.Add(ex);
            }
        }

        _tokens.Add(CreateToken(TokenKind.EOF));
        return (_tokens, _errors);
    }

    private Token? Lex()
    {
        char current = Peek();

        if (current == '\r' || current == '\n')
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

    private void LexLineComment()
    {
        while (Peek() != '\n' && !IsLastCharacter())
        {
            Consume();
        }

        CreateToken(TokenKind.LineComment);
    }

    private void LexNewLine()
    {
        if (Peek() == '\n')
        {
            _line++;
        }

        _column = 0;
        _location = new SourceLocation(_line, _column);

        Consume();
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

    private Token? LexPunctuation()
    {
        var current = Consume();
        switch (current)
        {
            case '*': return CreateToken(TokenKind.Star);
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
            case '[': return CreateToken(TokenKind.LeftBracket);
            case ']': return CreateToken(TokenKind.RightBracket);
            case '{': return CreateToken(TokenKind.LeftBrace);
            case '}': return CreateToken(TokenKind.RightBrace);
            case '%': return CreateToken(TokenKind.Percentage);

            case '/':
                if (Match('/'))
                {
                    LexLineComment();
                    return null;
                }
                return CreateToken(TokenKind.Slash);

            case '-':
                if (Match('-')) return CreateToken(TokenKind.MinusMinus);
                if (Match('=')) return CreateToken(TokenKind.MinusEqual);
                return CreateToken(TokenKind.Minus);

            case '+':
                if (Match('+')) return CreateToken(TokenKind.PlusPlus);
                if (Match('=')) return CreateToken(TokenKind.PlusEqual);
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
                CreateToken(TokenKind.Error);
                throw new SyntaxException(_location, $"The character '{current}' can't be tokenized");
        }
    }

    private Token? LexString()
    {
        Advance();
        while (Peek() != '"' && !IsLastCharacter())
        {
            if (Peek() == '\\')
            {
                Consume();
                HandleEscapeSequence();
            }
            else
            {
                Consume();
            }
        }

        Advance();
        return CreateToken(TokenKind.String);
    }

    private void LexWhiteSpace()
    {
        while (char.IsWhiteSpace(Peek()) && (Peek() is not '\r' or '\n') && !IsLastCharacter())
        {
            Consume();
        }

        CreateToken(TokenKind.WhiteSpace);
    }
}