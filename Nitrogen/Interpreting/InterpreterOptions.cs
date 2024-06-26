namespace Nitrogen.Interpreting;

public interface IOutputSink
{
    void Write(object? obj);

    void WriteLine(object? obj);
}

public sealed class ConsoleOutputSink : IOutputSink
{
    public void Write(object? obj) => Console.Write(obj);

    public void WriteLine(object? obj) => Console.WriteLine(obj);
}

public class InterpreterOptions
{
    public static readonly InterpreterOptions Default = new()
    {
        OutputSink = new ConsoleOutputSink(),
    };

    public IOutputSink OutputSink { get; set; } = new NullOutputSink();
}

public sealed class NullOutputSink : IOutputSink
{
    public void Write(object? obj)
    {
    }

    public void WriteLine(object? obj)
    {
    }
}

public sealed class TestOutputSink : IOutputSink
{
    public string? Result { get; set; }

    public void Write(object? obj) => Result += obj?.ToString();

    public void WriteLine(object? obj) => Result += obj?.ToString() + Environment.NewLine;
}