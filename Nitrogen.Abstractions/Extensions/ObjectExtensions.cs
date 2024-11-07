using Nitrogen.Abstractions.Interpreting.Declarations;

namespace Nitrogen.Abstractions.Extensions;

public static class ObjectExtensions
{
    // ToInternal for a single object
    public static object? ToInternal(this object? obj)
    {
        if (obj == null)
        {
            return null;
        }

        // Check for non-primitive types
        if (obj is not (long or float or decimal or int or byte or short or Enum))
        {
            var type = obj.GetType();

            // Handle arrays or classes
            return obj switch
            {
                IEnumerable<object> array => array.ToInternal(), // Recursively handle array
                _ when type.IsClass => new WrapperInstance(obj), // Wrap class instances
                _ => obj
            };
        }

        // Handle primitive types
        return obj switch
        {
            long or float or decimal or int or byte or short => Convert.ToDouble(obj),
            Enum => obj.ToString(), // Convert Enum to string
            _ => obj
        };
    }

    // ToInternal for arrays
    public static object? ToInternal(this IEnumerable<object> obj)
    {
        ICollection<object?> wrapped = [];

        foreach (var item in obj)
        {
            wrapped.Add(ToInternal(item));
        }

        return new WrapperInstance(wrapped);
    }

    public static object? Unwrap(this object? obj)
    {
        return obj switch
        {
            WrapperInstance wrap => wrap.Instance.Unwrap(),
            IEnumerable<object> array => array.Select(Unwrap).ToArray(),
            _ => obj,
        };
    }
}