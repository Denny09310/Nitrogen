using Nitrogen.Abstractions;
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
