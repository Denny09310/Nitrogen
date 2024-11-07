using Nitrogen.Abstractions.Interpreting;

namespace Nitrogen.Abstractions.Base;

public abstract class InstanceBase : IInstance
{
    public object? Get(Token member)
    {
        return Get(member.Lexeme);
    }

    public void Set(Token member, object? value)
    {
        Set(member.Lexeme, value);
    }

    protected abstract object? Get(string member);

    protected abstract void Set(string member, object? value);
}