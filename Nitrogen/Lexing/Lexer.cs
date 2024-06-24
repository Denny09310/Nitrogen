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
            case '+': return CreateToken(TokenKind.Plus);
            case '-': return CreateToken(TokenKind.Minus);
            case '*': return CreateToken(TokenKind.Star);
            case '/': return CreateToken(TokenKind.Slash);
            case ';': return CreateToken(TokenKind.Semicolon);
            default: throw new UnreachableException($"The character '{current}' can't be tokenized");
        }
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