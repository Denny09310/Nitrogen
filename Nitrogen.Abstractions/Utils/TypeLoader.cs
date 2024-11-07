using Nitrogen.Abstractions.Base;
using Nitrogen.Abstractions.Declarations;
using Nitrogen.Abstractions.Interpreting.Declarations;

namespace Nitrogen.Abstractions.Utils;

public static class TypeLoader
{
    public static IEnumerable<Type> FindClasses(Type type)
    {
        return type.Assembly.ExportedTypes.Where(IsNativeClass);
    }
    
    public static IEnumerable<Type> FindFunctions(Type type)
    {
        return type.Assembly.ExportedTypes.Where(IsNativeFunction);
    }

    private static bool IsNativeClass(Type type)
    {
        return !type.IsAbstract
            && type.Name != nameof(WrapperInstance)
            && typeof(NativeInstance).IsAssignableFrom(type);
    }

    private static bool IsNativeFunction(Type type)
    {
        return !type.IsAbstract
            && type.Name != nameof(MethodCallable)
            && type.Name != nameof(PropertyCallable)
            && !typeof(IFunctionDeclaration).IsAssignableFrom(type)
            && typeof(CallableBase).IsAssignableFrom(type);
    }
}
