namespace Nitrogen.Exceptions;
#pragma warning restore S3871 // Exception types should be "public"

#pragma warning disable S3871 // Exception types should be "public"

public class ReturnException(object? value) : Exception
{
    public object? Value { get; } = value;
}

#pragma warning restore S3871 // Exception types should be "public"