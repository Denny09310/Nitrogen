using Nitrogen.Syntax;

namespace Nitrogen.Interpreting.Declarations;

public interface IInstance
{
    object? Get(Token member);

    void Set(Token member, object? value);
}
