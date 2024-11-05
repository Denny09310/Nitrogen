using Nitrogen.Syntax;

namespace Nitrogen.Exceptions;

#pragma warning disable S3871 // Exception types should be "public"

public class RuntimeException : Exception
{
    public RuntimeException(string message) : base(message)
    {
    }

    public RuntimeException(Token token, string message) : base(message)
    {
        Token = token;
    }

    public Token Token { get; }
}

#pragma warning restore S3871 // Exception types should be "public"