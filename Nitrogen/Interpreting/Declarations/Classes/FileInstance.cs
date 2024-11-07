using Nitrogen.Abstractions.Interpreting.Declarations;

namespace Nitrogen.Interpreting.Declarations.Classes;

public class FileInstance : NativeInstance
{
    private static readonly Dictionary<string, MethodCallable> _methods = WrapMethods(typeof(File));
    private static readonly Dictionary<string, PropertyCallable> _properties = WrapProperties(typeof(File));

    public override Dictionary<string, MethodCallable> Methods => _methods;
    public override Dictionary<string, PropertyCallable> Properties => _properties;
    public override string Name => "file";
}
