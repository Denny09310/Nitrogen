namespace Nitrogen.Interpreting.Declarations;

public interface ICallable
{
    string Name { get; }

    object? Call(Interpreter interpreter, object?[] @params);

    void EnsureArity(object?[] @params);
}

public abstract class CallableBase : ICallable
{
    public abstract string Name { get; }

    public abstract object? Call(Interpreter interpreter, object?[] @params);

    public virtual void EnsureArity(object?[] @params)
    {
    }
}