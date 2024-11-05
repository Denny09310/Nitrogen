using Nitrogen.Exceptions;
using Nitrogen.Syntax;
using Nitrogen.Syntax.Statements;

namespace Nitrogen.Interpreting.Declarations;

public class ClassDeclaration(ClassStatement statement, ClassDeclaration? superclass, Dictionary<string, FunctionDeclaration> methods) : ICallable
{
    private FunctionDeclaration? _constructor;

    public Token Name { get; } = statement.Name;

    string ICallable.Name => Name.Lexeme;

    public virtual object? Call(Interpreter interpreter, object?[] @params)
    {
        var instance = new ClassInstance(this);
        _constructor?.Bind(instance).Call(interpreter, @params);
        return instance;
    }

    public virtual void EnsureArity(object?[] @params)
    {
        _constructor = FindMethod("constructor");
        if (_constructor == null) return;

        _constructor.EnsureArity(@params);
    }

    public FunctionDeclaration? FindMethod(string name)
    {
        if (methods.TryGetValue(name, out var method))
        {
            return method;
        }

        return superclass?.FindMethod(name);
    }

    public override string ToString() => $"class {Name.Lexeme} {{...}}";
}

public class ClassInstance(ClassDeclaration declaration) : InstanceBase
{
    private readonly Dictionary<string, object?> _fields = [];

    public override string ToString() => $"instanceof {declaration.Name.Lexeme}";

    protected override object? Get(string property)
    {
        if (_fields.TryGetValue(property, out var field))
        {
            return field;
        }

        if (declaration.FindMethod(property) is FunctionDeclaration method)
        {
            return method.Bind(this);
        }

        throw new RuntimeException($"The class '{declaration.Name.Lexeme}' has no property named '{property}'.");
    }

    protected override void Set(string property, object? value)
    {
        _fields[property] = value;
    }
}