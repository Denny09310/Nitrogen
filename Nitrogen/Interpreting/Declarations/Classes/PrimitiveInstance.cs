namespace Nitrogen.Interpreting.Declarations.Classes;

public class DoubleInstance : NativeInstance
{
    private static readonly Dictionary<string, MethodCallable> _methods = WrapMethods(typeof(double));
    private static readonly Dictionary<string, PropertyCallable> _properties = WrapProperties(typeof(double));

    public override Dictionary<string, MethodCallable> Methods => _methods;
    public override Dictionary<string, PropertyCallable> Properties => _properties;
    public override string Name => "double";
}

public class StringInstance : NativeInstance
{
    private static readonly Dictionary<string, MethodCallable> _methods = WrapMethods(typeof(string));
    private static readonly Dictionary<string, PropertyCallable> _properties = WrapProperties(typeof(string));

    public override Dictionary<string, MethodCallable> Methods => _methods;
    public override Dictionary<string, PropertyCallable> Properties => _properties;
    public override string Name => "string";
}

public class BoolInstance : NativeInstance
{
    private static readonly Dictionary<string, MethodCallable> _methods = WrapMethods(typeof(bool));
    private static readonly Dictionary<string, PropertyCallable> _properties = WrapProperties(typeof(bool));

    public override Dictionary<string, MethodCallable> Methods => _methods;
    public override Dictionary<string, PropertyCallable> Properties => _properties;
    public override string Name => "bool";
}