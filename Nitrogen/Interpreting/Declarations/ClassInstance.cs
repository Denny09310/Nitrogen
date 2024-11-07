using Nitrogen.Abstractions.Base;
using Nitrogen.Abstractions.Exceptions;

namespace Nitrogen.Interpreting.Declarations;

public class ClassInstance(ClassDeclaration declaration) : InstanceBase
{
    private readonly Dictionary<string, object?> _fields = [];

    public override string ToString() => $"instanceof {declaration.Name.Lexeme}";

    protected override object? Get(string member)
    {
        if (_fields.TryGetValue(member, out var field))
        {
            return field;
        }

        if (declaration.FindMethod(member) is FunctionDeclaration method)
        {
            return method.Bind(this);
        }

        throw new RuntimeException($"The class '{declaration.Name.Lexeme}' has no property named '{member}'.");
    }

    protected override void Set(string member, object? value)
    {
        _fields[member] = value;
    }
}