using Nitrogen.Core;
using Nitrogen.Core.Exceptions;
using Nitrogen.Core.Syntax.Expressions;
using Nitrogen.Core.Syntax.Expressions.Abstractions;
using Nitrogen.Core.Syntax.Statements;
using Nitrogen.Core.Syntax.Statements.Abstractions;

namespace Nitrogen.Parsing;

public partial class Parser(List<Token> tokens)
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

            return expression switch
            {
                IdentifierExpression identifier => new AssignmentExpression(identifier.Name, value),
                GetterExpression getter => new SetterExpression(getter.Name, getter.Expression, value),
                _ => throw new ParseException(equal, "Invalid assingment target.")
            };
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
        while (!Check(TokenKind.RightBrace))
        {
            statements.Add(ParseStatement());
        }

        Consume(TokenKind.RightBrace, "Expect '}' after block statement.");

        return new BlockStatement(statements);
    }

    private IExpression ParseCallExpression()
    {
        IExpression expression = ParsePrimaryExpression();

        while (true)
        {
            if (Match(TokenKind.LeftParenthesis))
            {
                var paren = Peek(-1);
                var parameters = ParseCallParameters(paren);

                expression = new CallExpression(paren, expression, parameters);
            }
            else if (Match(TokenKind.Dot))
            {
                var name = Consume(TokenKind.Identifier, "Expect property name after '.'.");
                expression = new GetterExpression(name, expression);
            }
            else if (Match(TokenKind.LeftBracket))
            {
                var bracket = Peek(-1);
                var index = ParseExpression();
                Consume(TokenKind.RightBracket, "Expect ']' after indexing expression.");
                expression = new IndexExpression(bracket, expression, index);
            }
            else
            {
                break;
            }
        }

        return expression;
    }

    private List<IExpression> ParseCallParameters(Token paren)
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

    private CatchStatement ParseCatchStatement()
    {
        var keyword = Peek(-1);

        IExpression? identifier = null;
        if (Match(TokenKind.LeftParenthesis))
        {
            identifier = ParseExpression();
            Consume(TokenKind.RightParenthesis, "Expect ')' after catch initializer.");

            if (identifier is not IdentifierExpression)
            {
                throw new ParseException(keyword, "The catch statement must have an identifier for the exception.");
            }
        }

        var body = ParseStatement();

        if (body is not BlockStatement)
        {
            throw new ParseException(keyword, "The catch statement must have a block body");
        }

        return new CatchStatement(keyword, identifier as IdentifierExpression, (BlockStatement)body);
    }

    private ClassStatement ParseClassStatement()
    {
        var name = Consume(TokenKind.Identifier, "Expect name after class statement.");

        IdentifierExpression? superclass = null;
        if (Match(TokenKind.Extends))
        {
            var supername = Consume(TokenKind.Identifier, "Expect superclass name after extends keyword.");
            superclass = new IdentifierExpression(supername);
        }

        Consume(TokenKind.LeftBrace, "Expect '{' after class name.");

        List<FunctionStatement> methods = [];
        while (!Check(TokenKind.RightBrace))
        {
            methods.Add(ParseFunctionStatement());
        }

        Consume(TokenKind.RightBrace, "Expect '}' after class declaration.");

        return new ClassStatement(name, superclass, methods);
    }

    private IExpression ParseComparisonExpression()
        => ParseBinaryExpression(ParseAdditiveExpression, TokenKind.Less, TokenKind.LessEqual, TokenKind.Greater, TokenKind.GreaterEqual);

    private IExpression ParseEqualityExpression()
        => ParseBinaryExpression(ParseComparisonExpression, TokenKind.EqualEqual, TokenKind.BangEqual);

    private IExpression ParseExpression()
    {
        return ParseAssignmentExpression();
    }

    private FinallyStatement ParseFinallyStatement()
    {
        var keyword = Peek(-1);
        var body = ParseStatement();
        return new FinallyStatement(keyword, body);
    }

    private ForStatement ParseForStatement()
    {
        var keyword = Peek(-1);
        Consume(TokenKind.LeftParenthesis, "Expect '(' after for statement.");

        IStatement? initialization = null;
        if (Match(TokenKind.Var))
        {
            initialization = ParseVariableStatement();
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
        if (!Check(TokenKind.RightParenthesis))
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

        Consume(TokenKind.RightParenthesis, "Expect ')' after function arguments.");
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

    private ImportStatement ParseImportStatement()
    {
        var keyword = Peek(-1);

        ICollection<IExpression> imports = [];
        do
        {
            var expression = ParseExpression();

            if (expression is not IdentifierExpression)
            {
                throw new ParseException(keyword, "Expect 'import name' after import statement.");
            }

            imports.Add(expression);
        }
        while (Match(TokenKind.Comma));

        Consume(TokenKind.From, "Expected 'from' keyword after import statement.");
        var source = ParseExpression();

        return new ImportStatement(imports, source);
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
        => ParseBinaryExpression(ParsePostfixExpression, TokenKind.Star, TokenKind.Slash);

    private IExpression ParseOrExpression()
        => ParseLogicalExpression(ParseAndExpression, TokenKind.PipePipe, TokenKind.Or);

    private IExpression ParsePostfixExpression()
    {
        var expression = ParsePrefixExpression();

        if (Match(TokenKind.PlusPlus, TokenKind.MinusMinus))
        {
            var @operator = Peek(-1);
            return new PostfixExpression(@operator, expression);
        }

        return expression;
    }

    private IExpression ParsePrefixExpression()
    {
        if (Match(TokenKind.PlusPlus, TokenKind.MinusMinus))
        {
            var @operator = Peek(-1);
            return new PrefixExpression(@operator, ParseExpression());
        }

        return ParseUnaryExpression();
    }

    private IExpression ParsePrimaryExpression()
    {
        Token current = Consume();

        if (current.Kind is TokenKind.True) return new LiteralExpression(true);
        if (current.Kind is TokenKind.False) return new LiteralExpression(false);

        if (current.Kind is TokenKind.Break) return new BreakExpression();
        if (current.Kind is TokenKind.Continue) return new ContinueExpression();
        if (current.Kind is TokenKind.This) return new ThisExpression(current);

        if (current.Kind is TokenKind.Super)
        {
            return ParseSuperExpression(current);
        }

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

        if (current.Kind is TokenKind.LeftBracket)
        {
            ICollection<IExpression> items = [];
            do
            {
                items.Add(ParseExpression());
            }
            while (Match(TokenKind.Comma));
            Consume(TokenKind.RightBracket, "Expect ']' after array expression.");
            return new ArrayExpression(items);
        }

        if (current.Kind is TokenKind.LeftBrace)
        {
            Dictionary<Token, IExpression> items = [];
            do
            {
                var key = Consume(TokenKind.String, "Expect key of type 'string'.");
                Consume(TokenKind.Colon, "Expect ':' separator after dictionary key.");
                var value = ParseExpression();

                if (value is not LiteralExpression)
                {
                    throw new ParseException(key, "Expect literal as entry value.");
                }

                items.Add(key, value);
            }
            while (Match(TokenKind.Comma));
            Consume(TokenKind.RightBrace, "Expect '}' after array expression.");
            return new DictionaryExpression(items);
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

    private IStatement ParseStatement()
    {
        if (Match(TokenKind.Function)) return ParseFunctionStatement();
        if (Match(TokenKind.Var)) return ParseVariableStatement();
        if (Match(TokenKind.Class)) return ParseClassStatement();
        if (Match(TokenKind.Import)) return ParseImportStatement();

        if (Match(TokenKind.While)) return ParseWhileStatement();
        if (Match(TokenKind.For)) return ParseForStatement();
        if (Match(TokenKind.If)) return ParseIfStatement();
        if (Match(TokenKind.Try)) return ParseTryStatement();
        if (Match(TokenKind.Catch)) return ParseCatchStatement();
        if (Match(TokenKind.Finally)) return ParseFinallyStatement();

        if (Match(TokenKind.LeftBrace)) return ParseBlockStatement();

        var expression = new ExpressionStatement(ParseExpression());
        Consume(TokenKind.Semicolon, "Expect ';' after statement.");

        return expression;
    }

    private SuperExpression ParseSuperExpression(Token current)
    {
        if (Match(TokenKind.LeftParenthesis))
        {
            var parameters = ParseCallParameters(current);
            return new SuperExpression(current, parameters);
        }

        Consume(TokenKind.Dot, "Expect '.' after super accessor.");
        Token member = Consume(TokenKind.Identifier, "Expect member name after super expression.");

        return new SuperExpression(current, member);
    }

    private TryStatement ParseTryStatement()
    {
        var keyword = Peek(-1);

        var body = ParseStatement();

        var @catch = Check(TokenKind.Catch)
            ? ParseStatement()
            : null;

        var @finally = Check(TokenKind.Finally)
            ? ParseStatement()
            : null;

        if (@catch == null && @finally == null)
        {
            throw new ParseException(keyword, "try statement must have either 'catch' or 'finally'.");
        }

        if (@catch != null && @catch is not CatchStatement)
        {
            throw new ParseException(keyword, "try statement must be followed by a 'catch' statement.");
        }

        if (@finally != null && @finally is not FinallyStatement)
        {
            throw new ParseException(keyword, "try statement must be followed by a 'finally' statement.");
        }

        return new TryStatement(keyword, body, @catch as CatchStatement, @finally as FinallyStatement);
    }

    private IExpression ParseUnaryExpression()
    {
        if (Match(TokenKind.Minus, TokenKind.Bang))
        {
            var @operator = Peek(-1);
            return new UnaryExpression(@operator, ParseExpression());
        }

        return ParseCallExpression();
    }

    private VarStatement ParseVariableStatement()
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