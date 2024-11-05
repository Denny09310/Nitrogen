using Nitrogen.Exceptions;
using Nitrogen.Interpreting.Declarations.Functions;

namespace Nitrogen.Interpreting.Declarations.Classes;

internal class ConsoleInstance : InstanceBase
{
    private static readonly Dictionary<string, object?> _fields = new()
    {
        ["print"] = new PrintFunction(),
        ["color"] = "Black",
    };

    protected override object? Get(string property)
    {
        return _fields[property];
    }

    protected override void Set(string property, object? value)
    {
        if (!_fields.ContainsKey(property))
        {
            throw new RuntimeException($"The class 'Console' has not field '{property}'");
        }

        _fields[property] = value;
    }
}