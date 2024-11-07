using Nitrogen.Core.Interpreting;
using Nitrogen.Core.Interpreting.Declarations;

namespace Nitrogen.Core.Utils;

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
        // Check if the class is not abstract, is not a WrapperInstance,
        // and is a subclass of NativeInstance (directly or indirectly).
        return !type.IsAbstract
            && !type.IsSubclassOf(typeof(WrapperInstance))  // Avoid classes that are subclasses of WrapperInstance
            && typeof(NativeInstance).IsAssignableFrom(type); // Check if it's a NativeInstance or its subclass
    }

    private static bool IsNativeFunction(Type type)
    {
        return !type.IsAbstract  // Ensure the type is not abstract (i.e., it can be instantiated).
            && type.Name != nameof(MethodCallable)  // Avoid types named 'MethodCallable' (likely the base for method calls).
            && type.Name != nameof(PropertyCallable)  // Avoid types named 'PropertyCallable' (likely the base for property calls).
            && !typeof(IFunctionDeclaration).IsAssignableFrom(type)  // Exclude types implementing IFunctionDeclaration.
            && !typeof(IClassDeclaration).IsAssignableFrom(type)  // Exclude types implementing IClassDeclaration.
            && typeof(ICallable).IsAssignableFrom(type);  // Ensure the type implements ICallable (native functions).
    }

}
