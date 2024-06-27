using Nitrogen.Exceptions;
using Nitrogen.Syntax;

namespace Nitrogen.Interpreting;

public class InterpreterEnvironment
{
    private readonly Dictionary<string, object?> _variables = [];

    public InterpreterEnvironment()
    {
    }

    public InterpreterEnvironment(InterpreterEnvironment enclosing)
    {
        Enclosing = enclosing;
    }

    public InterpreterEnvironment? Enclosing { get; private set; }

    public void Assign(Token name, object? value)
    {
        if (_variables.ContainsKey(name.Lexeme))
        {
            _variables[name.Lexeme] = value;
        }
        else if (Enclosing != null)
        {
            Enclosing.Assign(name, value);
        }
        else
        {
            throw new RuntimeException(name, $"Variable with name '{name.Lexeme}' not defined in this scope.");
        }
    }

    public void Define(Token name, object? value)
    {
        if (!_variables.TryAdd(name.Lexeme, value))
        {
            throw new RuntimeException(name, $"Variable with name '{name.Lexeme}' already defined in this scope.");
        }
    }

    public void Define(string name, object? value) => Define(new Token { Lexeme = name }, value);

    public object? Get(Token name)
    {
        if (_variables.TryGetValue(name.Lexeme, out var value)) return value;
        if (Enclosing is not null) return Enclosing.Get(name);
        throw new RuntimeException(name, $"Variable with name '{name.Lexeme}' not defined in this scope.");
    }

    public object? Get(string name) => Get(new Token { Lexeme = name });

    public object? GetAt(Token name, int distance)
    {
        return Ancestor(distance).Get(name);
    }

    internal void AssignAt(int distance, Token name, object? value)
    {
        Ancestor(distance).Assign(name, value);
    }

    private InterpreterEnvironment Ancestor(int distance)
    {
        var environment = this;
        for (int i = 0; i < distance; i++)
        {
            environment = environment.Enclosing ?? throw new RuntimeException("Can't lookup variable.");
        }
        return environment;
    }
}