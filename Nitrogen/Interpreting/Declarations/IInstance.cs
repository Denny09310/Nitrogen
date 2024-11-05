using Nitrogen.Syntax;

namespace Nitrogen.Interpreting.Declarations;

public interface IInstance
{
    object? Get(Token property);

    void Set(Token property, object? value);
}

public abstract class InstanceBase : IInstance
{
    public object? Get(Token property)
    {
        return Get(property.Lexeme);
    }

    public void Set(Token property, object? value)
    {
        Set(property.Lexeme, value);
    }

    protected abstract object? Get(string property);

    protected abstract void Set(string property, object? value);
}