namespace Nitrogen.Exceptions;

#pragma warning disable S3871 // Exception types should be "public"

internal class SyntaxException(SourceLocation location, string? message) : Exception(message)
{
    public SourceLocation Location { get; } = location;
}

#pragma warning restore S3871 // Exception types should be "public"