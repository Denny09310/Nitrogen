using Nitrogen.Interpreting.Declarations;

namespace Nitrogen.Extensions;

internal static class ObjectExtensions
{
    public static object? ToInternal(this object? obj)
    {
        var type = obj?.GetType();

        if (obj != null && type is { IsClass: true, IsPrimitive: false } && type != typeof(string))
        {
            return new WrapperInstance(obj);
        }

        return obj switch
        {
            long or float or decimal or int or byte or short => Convert.ToDouble(obj),
            Enum => obj.ToString(),
            _ => obj
        };
    }
}