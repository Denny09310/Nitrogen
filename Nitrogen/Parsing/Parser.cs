using Nitrogen.Syntax;
using Nitrogen.Syntax.Expressions;
using Nitrogen.Syntax.Expressions.Abstractions;

using System.Diagnostics;

namespace Nitrogen.Parsing;

internal partial class Parser(List<Token> tokens)
{
    private readonly List<IExpression> _expressions = [];

    private int _index;

    public List<IExpression> Parse()
    {
        while (!IsLastToken())
        {
            if (ParseExpression() is not IExpression expression) continue;
            _expressions.Add(expression);
        }

        return _expressions;
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
}

internal partial class Parser
{
    private bool Check(TokenKind kind) => Peek().Kind == kind;

    private Token Consume()
    {
        _index++;
        return Peek(-1);
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