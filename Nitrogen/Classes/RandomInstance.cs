using Nitrogen.Abstractions.Declarations;

namespace Nitrogen.Classes;

public class RandomInstance : NativeInstance
{
    private static readonly Dictionary<string, MethodCallable> _methods = WrapMethods(typeof(Random));
    private static readonly Dictionary<string, PropertyCallable> _properties = WrapProperties(typeof(Random));

    public override Dictionary<string, MethodCallable> Methods => _methods;
    public override Dictionary<string, PropertyCallable> Properties => _properties;
    public override string Name => "random";
}
