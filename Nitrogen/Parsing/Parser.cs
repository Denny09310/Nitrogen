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

    private IExpression ParseAndExpression()
        => ParseLogicalExpression(ParseAdditiveExpression, TokenKind.AmpersandAmpersand, TokenKind.And);

    private IExpression ParseAssignmentExpression()
    {
        var expression = ParseOrExpression();
        if (Match(TokenKind.Equal))
        {
            var value = ParseOrExpression();

            if (expression is IdentifierExpression identifier)
            {
                return new AssignmentExpression(identifier.Name, value);
            }

            throw new InvalidOperationException("Invalid assingment target.");
        }

        return expression;
    }

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
        var expression = ParseAssignmentExpression();
        return expression;
    }

    private ExpressionStatement ParseExpressionStatement()
    {
        var statement = new ExpressionStatement(ParseExpression());
        Consume(TokenKind.Semicolon, "Expect ';' after statement.");
        return statement;
    }

    private IExpression ParseLogicalExpression(Func<IExpression> descendant, params TokenKind[] kinds)
    {
        var left = descendant();
        while (Match(kinds))
        {
            var @operator = Peek(-1);
            var right = descendant();
            left = new LogicalExpression(left, @operator, right);
        }
        return left;
    }

    private IExpression ParseMoltiplicativeExpression()
        => ParseBinaryExpression(ParsePrimaryExpression, TokenKind.Star, TokenKind.Slash);

    private IExpression ParseOrExpression()
        => ParseLogicalExpression(ParseAndExpression, TokenKind.PipePipe, TokenKind.Or);

    private IExpression ParsePrimaryExpression()
    {
        Token current = Consume();

        if (current is { Kind: TokenKind.Number })
        {
            return new LiteralExpression(current.Value);
        }

        if (current is { Kind: TokenKind.Identifier })
        {
            return new IdentifierExpression(current);
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