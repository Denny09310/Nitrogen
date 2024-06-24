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
        => ParseLogicalExpression(ParseEqualityExpression, TokenKind.AmpersandAmpersand, TokenKind.And);

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

    private IExpression ParseComparisonExpression()
                    => ParseBinaryExpression(ParseAdditiveExpression, TokenKind.Less, TokenKind.LessEqual, TokenKind.Greater, TokenKind.GreaterEqual);

    private IExpression ParseEqualityExpression()
        => ParseBinaryExpression(ParseComparisonExpression, TokenKind.EqualEqual, TokenKind.BangEqual);

    private IExpression ParseExpression()
    {
        var expression = ParseAssignmentExpression();
        return expression;
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
        => ParseBinaryExpression(ParseUnaryExpression, TokenKind.Star, TokenKind.Slash);

    private IExpression ParseOrExpression()
        => ParseLogicalExpression(ParseAndExpression, TokenKind.PipePipe, TokenKind.Or);

    private IExpression ParsePrimaryExpression()
    {
        Token current = Consume();

        if (current.Kind is TokenKind.True) return new LiteralExpression(true);
        if (current.Kind is TokenKind.False) return new LiteralExpression(false);

        if (current.Kind is TokenKind.LeftParenthesis)
        {
            var expression = ParseExpression();
            var paren = Consume(TokenKind.RightParenthesis, "Expect ')' after grouping expression.");
            return new GroupingExpression(paren, expression);
        }

        if (current is { Kind: TokenKind.Number or TokenKind.String })
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
        return new ExpressionStatement(ParseExpression());
    }

    private IExpression ParseUnaryExpression()
    {
        IExpression? expression = null;
        if (Match(TokenKind.Minus, TokenKind.Bang))
        {
            var @operator = Peek(-1);
            expression = new UnaryExpression(@operator, ParseExpression());
        }

        return expression ?? ParsePrimaryExpression();
    }
}