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
        var expression = ParseCallExpression();
        if (Match(TokenKind.Equal))
        {
            var equal = Peek(-1);
            var value = ParseOrExpression();

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

    private BlockStatement ParseBlockStatement()
    {
        List<IStatement> statements = [];
        while (!Match(TokenKind.RightBrace))
        {
            statements.Add(ParseStatement());
        }

        return new BlockStatement(statements);
    }

    private List<IExpression> ParseCallArguments(Token paren)
    {
        List<IExpression> parameters = [];
        if (!Check(TokenKind.RightParenthesis))
        {
            do
            {
                if (parameters.Count >= 255)
                {
                    throw new ParseException(paren, "Can't have more that 254 arguments per function.");
                }

                parameters.Add(ParseExpression());
            }
            while (Match(TokenKind.Comma));
        }

        Consume(TokenKind.RightParenthesis, "Expect ')' after call arguments.");

        return parameters;
    }

    private IExpression ParseCallExpression()
    {
        IExpression expression = ParseOrExpression();
        if (Match(TokenKind.LeftParenthesis))
        {
            var paren = Peek(-1);
            var parameters = ParseCallArguments(paren);

            return new CallExpression(paren, expression, parameters);
        }

        return expression;
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
            initialization = ParseVariableDeclarationStatement();
        }
        else if (!Check(TokenKind.Semicolon))
        {
            initialization = new ExpressionStatement(ParseExpression());
            Consume(TokenKind.Semicolon, "Expect ';' after for initialization.");
        }

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

    private FunctionStatement ParseFunctionStatement()
    {
        var name = Consume(TokenKind.Identifier, "Expect name after function declaration.");
        Consume(TokenKind.LeftParenthesis, "Expect '(' after function name.");

        List<IExpression> arguments = [];
        if (!Check(TokenKind.RightParenthesis))
        {
            do
            {
                if (arguments.Count >= 255)
                {
                    throw new ParseException(name, "Can't have more that 254 arguments per function.");
                }

                var expression = ParseExpression();
                if (expression is not (AssignmentExpression or IdentifierExpression))
                {
                    throw new ParseException(name, "Arguments must be only identifiers or assignments.");
                }

                arguments.Add(expression);
            }
            while (Match(TokenKind.Comma));
        }

        Consume(TokenKind.RightParenthesis, "Expect '(' after function arguments.");
        Consume(TokenKind.LeftBrace, "Expect '{' after function declaration.");

        var body = ParseBlockStatement();

        return new FunctionStatement(name, arguments, body);
    }

    private IfStatement ParseIfStatement()
    {
        var keyword = Peek(-1);
        Consume(TokenKind.LeftParenthesis, "Expect '(' after if statement.");

        var condition = ParseExpression();
        Consume(TokenKind.RightParenthesis, "Expect ')' after if condition.");

        var then = ParseStatement();

        IStatement? @else = null;
        if (Match(TokenKind.Else))
        {
            @else = ParseStatement();
        }

        return new IfStatement(keyword, condition, then, @else);
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

        if (current.Kind is TokenKind.Break) return new BreakExpression();
        if (current.Kind is TokenKind.Continue) return new ContinueExpression();

        if (current.Kind is TokenKind.Return)
        {
            IExpression? value = null;
            if (!Check(TokenKind.Semicolon))
            {
                value = ParseExpression();
            }

            return new ReturnExpression(current, value);
        }

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
        if (Match(TokenKind.Function)) return ParseFunctionStatement();
        if (Match(TokenKind.Var)) return ParseVariableDeclarationStatement();

        if (Match(TokenKind.Print)) return ParsePrintStatement();
        if (Match(TokenKind.While)) return ParseWhileStatement();
        if (Match(TokenKind.For)) return ParseForStatement();
        if (Match(TokenKind.If)) return ParseIfStatement();

        if (Match(TokenKind.LeftBrace)) return ParseBlockStatement();

        var expression = new ExpressionStatement(ParseExpression());
        Consume(TokenKind.Semicolon, "Expect ';' after statement.");

        return expression;
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

    private VarStatement ParseVariableDeclarationStatement()
    {
        var name = Consume(TokenKind.Identifier, "Expect name after variable declaration.");

        IExpression? initializer = null;
        if (Match(TokenKind.Equal))
        {
            initializer = ParseExpression();
        }

        Consume(TokenKind.Semicolon, "Expect ';' after variable declaration statement.");

        return new VarStatement(name, initializer);
    }

    private WhileStatement ParseWhileStatement()
    {
        var keyword = Peek(-1);
        Consume(TokenKind.LeftParenthesis, "Expect '(' after while statement.");

        var condition = ParseExpression();
        Consume(TokenKind.RightParenthesis, "Expect ')' after while condition.");

        var body = ParseStatement();

        return new WhileStatement(keyword, condition, body);
    }
}