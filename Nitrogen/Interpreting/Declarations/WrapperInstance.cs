using Nitrogen.Extensions;

namespace Nitrogen.Interpreting.Declarations;

public class WrapperInstance(object instance) : NativeInstance
{
    private readonly string _name = instance.GetType().Name.ToSnakeCase();

    private Dictionary<string, MethodCallable>? _methods;
    private Dictionary<string, PropertyCallable>? _properties;

    public override Dictionary<string, MethodCallable> Methods => _methods ??= WrapMethods(instance.GetType());
    public override Dictionary<string, PropertyCallable> Properties => _properties ??= WrapProperties(instance.GetType());
    public override string Name => _name;

    protected override object? CallGetter(PropertyCallable getter) => getter.Bind(instance).Call(null!, []);

    protected override void CallSetter(PropertyCallable setter, object? value) => setter.Bind(instance).Call(null!, [value]);

    protected override MethodCallable CreateMethod(MethodCallable method) => method.Bind(instance);
}