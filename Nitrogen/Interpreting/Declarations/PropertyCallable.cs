using Nitrogen.Exceptions;
using Nitrogen.Extensions;
using System.Reflection;

namespace Nitrogen.Interpreting.Declarations;

public class PropertyCallable(PropertyInfo property) : CallableBase
{
    private readonly string _name = property.Name.ToSnakeCase();
    private readonly PropertyInfo _property = property;

    public override string Name => _name;

    public override object? Call(Interpreter interpreter, object?[] @params)
    {
        // Determine if this is a 'get' or 'set' operation based on parameter count
        if (@params.Length == 0) // Getter
        {
            return Get();
        }
        else if (@params.Length == 1) // Setter
        {
            Set(@params[0]);
            return null;
        }
        else
        {
            throw new RuntimeException($"Property '{_name}' requires either 0 (get) or 1 (set) parameter.");
        }
    }

    public override void EnsureArity(object?[] @params)
    {
        if (@params.Length != 0 && @params.Length != 1)
        {
            throw new RuntimeException($"Property '{_name}' requires either 0 (get) or 1 (set) parameter.");
        }
    }

    private object? Get()
    {
        if (!_property.CanRead)
        {
            throw new RuntimeException($"Property '{_name}' is not readable.");
        }

        return _property.GetValue(null);
    }

    private void Set(object? value)
    {
        if (!_property.CanWrite)
        {
            throw new RuntimeException($"Property '{_name}' is not writable.");
        }

        object? converted;

        if (_property.PropertyType.IsEnum)
        {
            if (value is string @string)
            {
                converted = Enum.Parse(_property.PropertyType, @string);
            }
            else if (value is double @double)
            {
                converted = Enum.ToObject(_property.PropertyType, (int)@double);
            }
            else
            {
                throw new RuntimeException($"Value '{value}' cannot be converted to enum '{_property.PropertyType}'.");
            }
        }
        else
        {
            try
            {
                converted = Convert.ChangeType(value, _property.PropertyType);
            }
            catch (Exception ex)
            {
                throw new RuntimeException($"Value of type '{_property.PropertyType}' cannot be assigned to property '{_name}' of type '{_property.PropertyType}'.", ex);
            }
        }

        _property.SetValue(null, converted);
    }
}