using Nitrogen.Core;
using Nitrogen.Core.Exceptions;

namespace Nitrogen.Parsing;

public partial class Parser
{
    private bool Check(TokenKind kind) => Peek().Kind == kind;

    private Token Consume()
    {
        _index++;
        return Peek(-1);
    }

    private Token Consume(TokenKind kind, string message)
    {
        var token = Peek();
        if (token.Kind == kind)
        {
            return Consume();
        }

        throw new ParseException(token, message);
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

    private void Synchronize()
    {
        Token current = Peek();
        while (!IsLastToken())
        {
            if (current.Kind is TokenKind.Semicolon or TokenKind.RightBrace or TokenKind.Class or TokenKind.Function or TokenKind.If or TokenKind.While or TokenKind.For)
            {
                break;
            }
            current = Consume();
        }
    }
}