namespace Nitrogen.Interpreting.Declarations;

public interface ICallable
{
    string Name { get; }

    object? Call(Interpreter interpreter, object?[] args);

    void Arity(object?[] args);
}
