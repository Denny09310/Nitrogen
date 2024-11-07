using Nitrogen.Abstractions.Extensions;

namespace Nitrogen.Abstractions.Declarations;

public class WrapperInstance(object instance) : NativeInstance
{
    private readonly string _name = instance.GetType().Name.ToSnakeCase();

    private Dictionary<string, MethodCallable>? _methods;
    private Dictionary<string, PropertyCallable>? _properties;

    public object Instance { get; } = instance;
    public override string Name => _name;

    public override Dictionary<string, MethodCallable> Methods => _methods ??= WrapMethods(Instance.GetType());
    public override Dictionary<string, PropertyCallable> Properties => _properties ??= WrapProperties(Instance.GetType());

    protected override object? CallGetter(PropertyCallable getter) => getter.Bind(Instance).Call(null!, []);

    protected override void CallSetter(PropertyCallable setter, object? value) => setter.Bind(Instance).Call(null!, [value]);

    protected override MethodCallable CreateMethod(MethodCallable method) => method.Bind(Instance);
}