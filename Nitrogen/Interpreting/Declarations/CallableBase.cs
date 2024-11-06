namespace Nitrogen.Interpreting.Declarations;

public abstract class CallableBase : ICallable
{
    public abstract string Name { get; }

    public abstract object? Call(Interpreter interpreter, object?[] args);

    public virtual void Arity(object?[] args)
    {
    }
}