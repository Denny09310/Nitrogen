using Nitrogen.Abstractions.Exceptions;
using Nitrogen.Abstractions.Extensions;
using Nitrogen.Extensions;
using System.Reflection;

namespace Nitrogen.Abstractions.Interpreting.Declarations;

public class PropertyCallable(PropertyInfo property) : ICallable
{
    private readonly string _name = property.Name.ToSnakeCase();
    private readonly PropertyInfo _property = property;

    private object? _instance;

    public string Name => _name;

    public void Arity(object?[] args)
    {
        if (args.Length != 0 && args.Length != 1)
        {
            throw new RuntimeException($"Property '{_name}' requires either 0 (get) or 1 (set) parameter.");
        }
    }

    public PropertyCallable Bind(object instance)
    {
        _instance = instance;
        return this;
    }

    public object? Call(IInterpreter interpreter, object?[] args)
    {
        if (args.Length == 0)
        {
            return Get();
        }
        else if (args.Length == 1)
        {
            Set(args[0]);
            return null;
        }
        else
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

        return _property.GetValue(_instance).ToInternal();
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

        _property.SetValue(_instance, converted);
    }
}