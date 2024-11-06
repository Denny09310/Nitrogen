using Nitrogen.Syntax;

namespace Nitrogen.Exceptions;

public class RuntimeException : Exception
{
    public RuntimeException(string? message) : base(message)
    {
    
    }

    public RuntimeException(string? message, Exception? innerException) : base(message, innerException)
    {
    }


    public RuntimeException(Token token, string message) : base(message)
    {
        Token = token;
    }

    public Token Token { get; }
}