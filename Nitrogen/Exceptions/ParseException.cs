using Nitrogen.Syntax;

namespace Nitrogen.Exceptions;

#pragma warning disable S3871 // Exception types should be "public"

internal class ParseException(Token token, string? message) : Exception(message)
{
    public Token Token { get; } = token;
}

#pragma warning restore S3871 // Exception types should be "public"