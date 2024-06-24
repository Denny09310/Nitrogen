﻿using Nitrogen.Syntax;
using System.Diagnostics;
using System.Text;

namespace Nitrogen;

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
        var current = Consume();

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
            return LexInteger();
        }

        if (char.IsLetter(current) || current == '_')
        {
            return LexIdentifier();
        }

        return LexPunctuation(current);
    }

    private Token LexFloat()
    {
        while ((!char.IsDigit(Peek()) || Peek() is '.') && !IsLastCharacter())
        {
            Consume();
        }

        return CreateToken(TokenKind.Float);
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

    private Token LexInteger()
    {
        while (char.IsDigit(Peek()) && !IsLastCharacter())
        {
            Consume();
        }

        if (Peek() is not '.')
        {
            return CreateToken(TokenKind.Integer);
        }

        return LexFloat();
    }

    private void LexNewLine()
    {
        AdvanceNewLine();
        CreateToken(TokenKind.NewLine);
    }

    private Token LexPunctuation(char current)
    {
        switch (current)
        {
            case '+': return CreateToken(TokenKind.Plus);
            case '-': return CreateToken(TokenKind.Minus);
            case '*': return CreateToken(TokenKind.Star);
            case '/': return CreateToken(TokenKind.Slash);
			case '\\': return CreateToken(TokenKind.BackSlash);
			case '(': return CreateToken(TokenKind.LeftParenthesis);
			case ')': return CreateToken(TokenKind.RightParenthesis);
			case '!': return CreateToken(TokenKind.Exclamation);
			case '?': return CreateToken(TokenKind.Question);
			case '=': return CreateToken(TokenKind.Equal);
			case '<': return CreateToken(TokenKind.Less);
			case '>': return CreateToken(TokenKind.Greater);
			case '.': return CreateToken(TokenKind.Dot);
			case ',': return CreateToken(TokenKind.Comma);
			case ';': return CreateToken(TokenKind.Semicolon);
			case ':': return CreateToken(TokenKind.Colon);
			case '"': return CreateToken(TokenKind.Quote);
			case '\'': return CreateToken(TokenKind.SingleQuote);
			case '[': return CreateToken(TokenKind.SquareLeft);
			case ']': return CreateToken(TokenKind.SquareRight);
			case '{': return CreateToken(TokenKind.CurlyLeft);
			case '}': return CreateToken(TokenKind.CurlyRight);
			case '%': return CreateToken(TokenKind.Percentage);
			case '&': return CreateToken(TokenKind.Ampersand);
			case '|': return CreateToken(TokenKind.VerticalBar);

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