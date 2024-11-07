namespace Nitrogen.Abstractions.Interpreting;

public interface IInstance
{
    object? Get(Token member);

    void Set(Token member, object? value);
}
