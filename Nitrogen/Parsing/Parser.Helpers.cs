using Nitrogen.Syntax;

namespace Nitrogen.Parsing;

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