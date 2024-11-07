using Nitrogen.Interpreting.Declarations;

namespace Nitrogen.Extensions;

internal static class ObjectExtensions
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
                Array array => array.ToInternal(), // Recursively handle array
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
    public static object? ToInternal(this Array obj)
    {
        var wrappedArray = new object?[obj.Length];

        for (int i = 0; i < obj.Length; i++)
        {
            wrappedArray[i] = obj.GetValue(i).ToInternal(); // Use GetValue for array element access
        }

        return new WrapperInstance(wrappedArray);
    }

    public static object? Unwrap(this object? obj)
    {
        return obj switch
        {
            WrapperInstance wrap => wrap.Instance.Unwrap(),
            object[] array => array.Select(Unwrap).ToArray(),
            _ => obj,
        };
    }
}