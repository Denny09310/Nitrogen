using Nitrogen.Abstractions.Interpreting;

namespace Nitrogen.Abstractions.Base;

public abstract class CallableBase : ICallable
{
    public abstract string Name { get; }

    public abstract object? Call(IInterpreter interpreter, object?[] args);

    public virtual void Arity(object?[] args)
    {
    }
}