using Nitrogen.Syntax;

namespace Nitrogen.Exceptions;

#pragma warning disable S3871 // Exception types should be "public"

internal class BindingException(ExceptionLevel level, Token token, string message) : Exception(message)
{
    public ExceptionLevel Level { get; } = level;
    public Token Token { get; } = token;
}

internal enum ExceptionLevel
{
    None,
    Info,
    Warning,
    Error,
}

#pragma warning restore S3871 // Exception types should be "public"