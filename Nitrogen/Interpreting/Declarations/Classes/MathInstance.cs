using Nitrogen.Core.Interpreting.Declarations;

namespace Nitrogen.Interpreting.Declarations.Classes;

public class MathInstance : NativeInstance
{
    private static readonly Dictionary<string, MethodCallable> _methods = WrapMethods(typeof(Math));
    private static readonly Dictionary<string, PropertyCallable> _properties = WrapProperties(typeof(Math));

    public override Dictionary<string, MethodCallable> Methods => _methods;
    public override Dictionary<string, PropertyCallable> Properties => _properties;
    public override string Name => "math";
}