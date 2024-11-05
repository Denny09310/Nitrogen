using Nitrogen.Exceptions;
using Nitrogen.Syntax;
using Nitrogen.Syntax.Statements;

namespace Nitrogen.Interpreting.Declarations;

public class ClassDeclaration(ClassStatement statement, ClassDeclaration? superclass, Dictionary<string, FunctionDeclaration> methods) : ICallable
{
    private FunctionDeclaration? _constructor;

    public Token Name { get; } = statement.Name;
    string ICallable.Name => Name.Lexeme;

    public void Arity(object?[] @params)
    {
        _constructor = FindMethod("constructor");
        if (_constructor == null) return;

        _constructor.Arity(@params);
    }

    public object? Call(Interpreter interpreter, object?[] @params)
    {
        var instance = new ClassInstance(this);
        _constructor?.Bind(instance).Call(interpreter, @params);
        return instance;
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

public class ClassInstance(ClassDeclaration declaration)
{
    private readonly Dictionary<string, object?> _fields = [];

    public object? Get(Token property)
    {
        if (_fields.TryGetValue(property.Lexeme, out var field))
        {
            return field;
        }

        if (declaration.FindMethod(property.Lexeme) is FunctionDeclaration method)
        {
            return method.Bind(this);
        }

        throw new RuntimeException(property, $"The class '{declaration.Name.Lexeme}' has no property named '{property.Lexeme}'.");
    }

    public void Set(Token property, object? value)
    {
        _fields[property.Lexeme] = value;
    }

    public override string ToString() => $"instanceof {declaration.Name.Lexeme}";
}