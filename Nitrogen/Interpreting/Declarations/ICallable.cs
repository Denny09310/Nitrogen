namespace Nitrogen.Interpreting.Declarations;

public interface ICallable
{
    string Name { get; }

    object? Call(Interpreter interpreter, object?[] args);

    void Arity(object?[] args);
}

public abstract class CallableBase : ICallable
{
    public abstract string Name { get; }

    public abstract object? Call(Interpreter interpreter, object?[] args);

    public virtual void Arity(object?[] args)
    {
    }
}