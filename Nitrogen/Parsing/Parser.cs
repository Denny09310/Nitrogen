using Nitrogen.Exceptions;
using Nitrogen.Syntax;
using Nitrogen.Syntax.Abstractions;
using Nitrogen.Syntax.Expressions;
using Nitrogen.Syntax.Statements;

namespace Nitrogen.Parsing;

internal partial class Parser(List<Token> tokens)
{
    private readonly List<ParseException> _errors = [];
    private readonly List<IStatement> _statements = [];
    private int _index;

    public (List<IStatement>, List<ParseException>) Parse()
    {
        while (!IsLastToken())
        {
            try
            {
                if (ParseStatement() is not IStatement statement) continue;
                _statements.Add(statement);
            }
            catch (ParseException ex)
            {
                _errors.Add(ex);
                Synchronize();
            }
        }

        return (_statements, _errors);
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
            var equal = Peek(-1);
            var value = ParseOrExpression();

            Consume(TokenKind.Semicolon, "Expect ';' after assignment.");

            if (expression is IdentifierExpression identifier)
            {
                return new AssignmentExpression(identifier.Name, value);
            }

            throw new ParseException(equal, "Invalid assingment target.");
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

    private ForStatement ParseForStatement()
    {
        var keyword = Peek(-1);
        Consume(TokenKind.LeftParenthesis, "Expect '(' after for statement.");

        IStatement? initialization = null;
        if (Match(TokenKind.Var))
        {
            // TODO: Add variable parsing
        }
        else if (!Check(TokenKind.Semicolon))
        {
            initialization = new ExpressionStatement(ParseExpression());
        }

        Consume(TokenKind.Semicolon, "Expect ';' after for initialization.");

        IExpression condition = new LiteralExpression(true);
        if (!Check(TokenKind.Semicolon))
        {
            condition = ParseExpression();
        }

        Consume(TokenKind.Semicolon, "Expect ';' after for condition.");

        IExpression? increment = null;
        if (!Check(TokenKind.Semicolon))
        {
            increment = ParseExpression();
        }

        Consume(TokenKind.RightParenthesis, "Expect ')' after for increment.");

        var body = ParseStatement();

        return new ForStatement(keyword, initialization, condition, body, increment);
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

        throw new ParseException(current, $"Token '{current.Lexeme}' not recognized.");
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
        if (Match(TokenKind.While)) return ParseWhileStatement();
        if (Match(TokenKind.For)) return ParseForStatement();
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

    private WhileStatement ParseWhileStatement()
    {
        var keyword = Peek(-1);
        Consume(TokenKind.LeftParenthesis, "Expect '(' after while statement.");

        var condition = ParseExpression();
        Consume(TokenKind.RightParenthesis, "Expect ')' after while condition.");

        Consume(TokenKind.LeftBrace, "Expect '{' after while condition.");
        var body = ParseStatement();

        Consume(TokenKind.RightBrace, "Expect '}' after while body");
        return new WhileStatement(keyword, condition, body);
    }
}