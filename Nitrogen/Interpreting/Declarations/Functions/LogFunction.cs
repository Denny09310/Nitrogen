namespace Nitrogen.Interpreting.Declarations.Functions;

public class LogFunction : CallableBase
{
    public override string Name => "log";

    public override object? Call(Interpreter interpreter, object?[] @params)
    {
        Console.WriteLine($"{@params[0]}", @params[1..]);

        return null;
    }
}