using Nitrogen.Abstractions;
using Nitrogen.Abstractions.Base;
using Nitrogen.Abstractions.Declarations;
using Nitrogen.Abstractions.Exceptions;
using Nitrogen.Abstractions.Interpreting;
using Nitrogen.Abstractions.Syntax.Statements;

namespace Nitrogen.Interpreting.Declarations;

public class ClassDeclaration(ClassStatement statement, ClassDeclaration? superclass, Dictionary<string, FunctionDeclaration> methods) : ICallable
{
    private FunctionDeclaration? _constructor;

    public Token Name { get; } = statement.Name;

    string ICallable.Name => Name.Lexeme;

    public virtual object? Call(IInterpreter interpreter, object?[] args)
    {
        var instance = new ClassInstance(this);
        _constructor?.Bind(instance).Call(interpreter, args);
        return instance;
    }

    public virtual void Arity(object?[] args)
    {
        _constructor = FindMethod("constructor");
        if (_constructor == null) return;

        _constructor.Arity(args);
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