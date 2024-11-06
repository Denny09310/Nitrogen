using Nitrogen.Exceptions;
using Nitrogen.Extensions;
using System.Reflection;

namespace Nitrogen.Interpreting.Declarations;

public partial class MethodCallable(string name, List<MethodInfo> overloads) : CallableBase
{
    private readonly string _name = name.ToSnakeCase();
    private readonly List<MethodInfo> _overloads = overloads;

    private object? _instance;

    public override string Name => _name;

    public override void Arity(object?[] args)
    {
        if (!_overloads.Exists(m => m.GetParameters().Length == args.Length))
        {
            throw new RuntimeException($"No overload of '{_name}' accepts {args.Length} parameters.");
        }
    }

    public MethodCallable Bind(object instance)
    {
        _instance = instance;
        return this;
    }

    public override object? Call(Interpreter interpreter, object?[] args)
    {
        args.Unwrap();

        // Select the overload based on the number of parameters
        var method = _overloads.Find(m => IsMatchingOverload(m, args))
            ?? throw new RuntimeException($"No overload found for method '{_name}' with {args.Length} parameters.");

        // Invoke the selected method
        var result = method.Invoke(_instance, args);
        return result.ToInternal();
    }

    private static bool IsMatchingOverload(MethodInfo method, object?[] args)
    {
        var parameters = method.GetParameters();

        // Check if parameter count matches
        if (parameters.Length != args.Length) return false;

        // Check if each parameter type is compatible with the corresponding argument type
        for (int i = 0; i < parameters.Length; i++)
        {
            var paramType = parameters[i].ParameterType;
            var argType = args[i]?.GetType();

            // Handle null arguments
            if (argType == null)
            {
                // Null is compatible with reference types or nullable value types
                if (!paramType.IsClass && Nullable.GetUnderlyingType(paramType) == null)
                {
                    return false;
                }
            }
            else if (argType == typeof(double))
            {
                if (!TryConvertArgument(args, i, paramType))
                {
                    return false;
                }
            }
            else if (!paramType.IsAssignableFrom(argType))
            {
                // Argument type is not compatible
                return false;
            }
        }

        return true;
    }

    private static bool TryConvertArgument(object?[] args, int index, Type targetType)
    {
        try
        {
            if (targetType == typeof(float) || targetType == typeof(int) || targetType == typeof(long) || targetType == typeof(decimal) || targetType == typeof(double))
            {
                args[index] = Convert.ChangeType(args[index], targetType);
                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }
}