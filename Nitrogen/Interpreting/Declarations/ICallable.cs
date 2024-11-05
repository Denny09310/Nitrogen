namespace Nitrogen.Interpreting.Declarations;

public interface ICallable
{
    string Name { get; }

    void Arity(object?[] @params);

    object? Call(Interpreter interpreter, object?[] @params);
}