using Nitrogen.Extensions;
using System.Reflection;

namespace Nitrogen.Interpreting.Declarations;

public partial class MethodCallable(string name, List<MethodInfo> overloads) : CallableBase
{
    private readonly string _name = name.ToSnakeCase();
    private readonly List<MethodInfo> _overloads = overloads;

    public override string Name => _name;

    public override object? Call(Interpreter interpreter, object?[] @params)
    {
        // Select the overload based on the number of parameters
        var method = _overloads.Find(m => IsMatchingOverload(m, @params))
            ?? throw new ArgumentException($"No overload found for method '{_name}' with {@params.Length} parameters.");

        // Invoke the selected method
        return method.Invoke(null, @params);
    }

    public override void EnsureArity(object?[] @params)
    {
        if (!_overloads.Exists(m => m.GetParameters().Length == @params.Length))
        {
            throw new ArgumentException($"No overload of '{_name}' accepts {@params.Length} parameters.");
        }
    }

    private static bool IsMatchingOverload(MethodInfo method, object?[] @params)
    {
        var parameters = method.GetParameters();

        // Check if parameter count matches
        if (parameters.Length != @params.Length) return false;

        // Check if each parameter type is compatible with the corresponding argument type
        for (int i = 0; i < parameters.Length; i++)
        {
            var paramType = parameters[i].ParameterType;
            var argType = @params[i]?.GetType();

            // Handle null arguments
            if (argType == null)
            {
                // Null is compatible with reference types or nullable value types
                if (!paramType.IsClass && Nullable.GetUnderlyingType(paramType) == null)
                    return false;
            }
            else if (!paramType.IsAssignableFrom(argType))
            {
                // Argument type is not compatible
                return false;
            }
        }

        return true;
    }
}