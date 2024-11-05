using Nitrogen.Exceptions;

namespace Nitrogen.Interpreting.Declarations.Functions;

public class PrintFunction : ICallable
{
    public string Name { get; } = "print";

    public object? Call(Interpreter interpreter, object?[] @params)
    {
        Console.WriteLine(@params[0]);
        return null;
    }

    public void EnsureArity(object?[] @params)
    {
        if (@params.Length != 1)
        {
            throw new RuntimeException("'print' must have only one argument.");
        }
    }
}