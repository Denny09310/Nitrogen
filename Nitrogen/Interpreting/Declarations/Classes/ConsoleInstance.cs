using Nitrogen.Abstractions.Interpreting.Declarations;

namespace Nitrogen.Interpreting.Declarations.Classes;

public class ConsoleInstance : NativeInstance
{
    private static readonly Dictionary<string, MethodCallable> _methods = WrapMethods(typeof(Console));
    private static readonly Dictionary<string, PropertyCallable> _properties = WrapProperties(typeof(Console));

    public override string Name => "console";
    public override Dictionary<string, MethodCallable> Methods => _methods;
    public override Dictionary<string, PropertyCallable> Properties => _properties;
}