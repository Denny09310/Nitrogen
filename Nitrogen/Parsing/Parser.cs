using Nitrogen.Syntax;
using Nitrogen.Syntax.Abstractions;
using Nitrogen.Syntax.Expressions;
using Nitrogen.Syntax.Statements;
using System.Diagnostics;

namespace Nitrogen.Parsing;

internal partial class Parser(List<Token> tokens)
{
    private readonly List<IStatement> _statements = [];

    private int _index;

    public List<IStatement> Parse()
    {
        while (!IsLastToken())
        {
            if (ParseStatement() is not IStatement statement) continue;
            _statements.Add(statement);
        }

        return _statements;
    }

    private IExpression ParseAdditiveExpression()
        => ParseBinaryExpression(ParseMoltiplicativeExpression, TokenKind.Plus, TokenKind.Minus);

    private IExpression ParseBinaryExpression(Func<IExpression> descendant, params TokenKind[] kinds)
    {
        var left = descendant();
        while (Match(kinds))
        {
            var @operator = Peek(-1);
            var right = descendant();
            left = new BinaryExpression(left, @operator, right);
        }
        return left;
    }

    private IExpression ParseExpression()
    {
        var expression = ParseAdditiveExpression();
        return expression;
    }

    private ExpressionStatement ParseExpressionStatement()
    {
        var statement = new ExpressionStatement(ParseExpression());
        Consume(TokenKind.Semicolon, "Expect ';' after statement.");
        return statement;
    }

    private IExpression ParseMoltiplicativeExpression()
        => ParseBinaryExpression(ParsePrimaryExpression, TokenKind.Star, TokenKind.Slash);

    private IExpression ParsePrimaryExpression()
    {
        Token current = Consume();

        if (current is { Kind: TokenKind.Number })
        {
            return new LiteralExpression(current.Value);
        }

        throw new UnreachableException($"Token '{current.Lexeme}' not recognized.");
    }

    private PrintStatement ParsePrintStatement()
    {
        var expression = ParseExpression();
        Consume(TokenKind.Semicolon, "Expect ';' after statement.");
        return new PrintStatement(expression);
    }

    private IStatement ParseStatement()
    {
        if (Match(TokenKind.Print)) return ParsePrintStatement();
        return ParseExpressionStatement();
    }
}

internal partial class Parser
{
    private bool Check(TokenKind kind) => Peek().Kind == kind;

    private Token Consume()
    {
        _index++;
        return Peek(-1);
    }

    private void Consume(TokenKind kind, string message)
    {
        if (Peek().Kind == kind)
        {
            Consume();
            return;
        }

        throw new InvalidOperationException(message);
    }

    private bool IsLastToken() => tokens[_index] is { Kind: TokenKind.EOF };

    private bool Match(params TokenKind[] kinds)
    {
        if (Array.Exists(kinds, Check))
        {
            Consume();
            return true;
        }

        return false;
    }

    private Token Peek(int count = 0) => tokens[_index + count];
}