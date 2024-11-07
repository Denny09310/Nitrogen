namespace Nitrogen.Abstractions.Interpreting;

public interface ICallable
{
    string Name { get; }

    object? Call(IInterpreter interpreter, object?[] args);

    void Arity(object?[] args);
}
