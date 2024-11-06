using System.Text.RegularExpressions;

namespace Nitrogen.Extensions;

internal static partial class StringExtensions
{
    public static string ToSnakeCase(this string name)
    {
        return GetTextFormatter().Replace(name, "_$1").ToLower();
    }

    [GeneratedRegex("(?<!^)([A-Z])", RegexOptions.Compiled)]
    private static partial Regex GetTextFormatter();
}