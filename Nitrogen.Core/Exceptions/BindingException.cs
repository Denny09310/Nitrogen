using Nitrogen.Core;

namespace Nitrogen.Core.Exceptions;

#pragma warning disable S3871 // Exception types should be "public"

public class BindingException(ExceptionLevel level, Token token, string message) : Exception(message)
{
    public ExceptionLevel Level { get; } = level;
    public Token Token { get; } = token;
}

public enum ExceptionLevel
{
    None,
    Info,
    Warning,
    Error,
}

#pragma warning restore S3871 // Exception types should be "public"