namespace Nitrogen.Core.Interpreting;

public interface IFunctionDeclaration : ICallable
{
    IFunctionDeclaration Bind(IClassInstance instance);
}
