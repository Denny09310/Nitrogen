using Nitrogen.Exceptions;
using Nitrogen.Extensions;
using System.Reflection;

namespace Nitrogen.Interpreting.Declarations;

public abstract class NativeInstance : InstanceBase
{
    public abstract Dictionary<string, MethodCallable> Methods { get; }
    public abstract Dictionary<string, PropertyCallable> Properties { get; }
    public abstract string Name { get; }

    protected static Dictionary<string, MethodCallable> WrapMethods(Type type)
    {
        return type
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
            .Where(m => !m.IsSpecialName) // Exclude property getters/setters
            .Where(m => !Array.Exists(m.GetParameters(), p => p.IsOut || p.ParameterType.IsByRef || p.IsIn)) // Exclude ref, out, and in parameters
            .Where(m => !typeof(Task).IsAssignableFrom(m.ReturnType)) // Exclude async methods
            .GroupBy(m => m.Name)
            .ToDictionary(
                m => m.Key.ToSnakeCase(),
                m => new MethodCallable(m.Key, new(m)));
    }

    protected static Dictionary<string, PropertyCallable> WrapProperties(Type type)
    {
        return type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
            .ToDictionary(
                p => p.Name.ToSnakeCase(),
                p => new PropertyCallable(p));
    }

    protected virtual object? CallGetter(PropertyCallable getter) => getter.Call(null!, []);

    protected virtual void CallSetter(PropertyCallable setter, object? value) => setter.Call(null!, [value]);

    protected virtual MethodCallable CreateMethod(MethodCallable method) => method;

    protected override object? Get(string member)
    {
        if (Properties.TryGetValue(member, out var getter))
        {
            // We can pass null as the interpreter is not used for PropertyCallable
            return CallGetter(getter);
        }

        if (Methods.TryGetValue(member, out var method))
        {
            return CreateMethod(method);
        }

        throw new RuntimeException($"Property or method '{member}' not found.");
    }

    protected override void Set(string member, object? value)
    {
        if (!Properties.TryGetValue(member, out PropertyCallable? setter))
        {
            throw new RuntimeException($"The class '{Name}' has not field '{member}'");
        }

        // We can pass null as the interpreter is not used for PropertyCallable
        CallSetter(setter, value);
    }
}