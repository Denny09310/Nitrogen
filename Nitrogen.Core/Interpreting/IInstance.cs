namespace Nitrogen.Core.Interpreting;

public interface IInstance
{
    object? Get(Token member);

    void Set(Token member, object? value);
}
