namespace Nitrogen.Abstractions.Interpreting;

public interface IFunctionDeclaration : ICallable
{
    IFunctionDeclaration Bind(IClassInstance instance);
}
