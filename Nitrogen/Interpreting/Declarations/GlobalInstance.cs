using Nitrogen.Exceptions;
using Nitrogen.Extensions;
using System.Reflection;

namespace Nitrogen.Interpreting.Declarations;

public abstract class GlobalInstance(Interpreter interpreter) : InstanceBase
{
    private readonly Interpreter _interpreter = interpreter;

    public abstract Dictionary<string, MethodCallable> Methods { get; }
    public abstract Dictionary<string, PropertyCallable> Properties { get; }

    public abstract string Name { get; }

    protected static Dictionary<string, MethodCallable> WrapMethods(Type type)
    {
        return type
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
            .Where(m => !m.IsSpecialName) // Exclude property getters/setters
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

    protected override object? Get(string member)
    {
        if (Properties.TryGetValue(member, out var getter))
        {
            return getter.Call(_interpreter, []);
        }

        if (Methods.TryGetValue(member, out var method))
        {
            return method;
        }

        throw new RuntimeException($"Property or method '{member}' not found.");
    }

    protected override void Set(string member, object? value)
    {
        if (!Properties.TryGetValue(member, out PropertyCallable? setter))
        {
            throw new RuntimeException($"The class '{Name}' has not field '{member}'");
        }

        setter.Call(_interpreter, [value]);
    }
}
