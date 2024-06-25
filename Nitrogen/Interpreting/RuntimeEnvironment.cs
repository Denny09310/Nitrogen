using Nitrogen.Exceptions;
using Nitrogen.Syntax;

namespace Nitrogen.Interpreting;

internal class RuntimeEnvironment
{
    private readonly Dictionary<string, object?> _variables = [];

    public RuntimeEnvironment()
    {
    }

    public RuntimeEnvironment(RuntimeEnvironment enclosing)
    {
        Enclosing = enclosing;
    }

    public RuntimeEnvironment? Enclosing { get; private set; }

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

    public object? Get(Token name)
    {
        if (_variables.TryGetValue(name.Lexeme, out var value)) return value;
        if (Enclosing is not null) return Enclosing.Get(name);
        throw new RuntimeException(name, $"Variable with name '{name.Lexeme}' not defined in this scope.");
    }

    public object? GetAt(Token name, int distance)
    {
        return Ancestor(distance).Get(name);
    }

    internal void AssignAt(int distance, Token name, object? value)
    {
        Ancestor(distance).Assign(name, value);
    }

    private RuntimeEnvironment Ancestor(int distance)
    {
        var environment = this;
        for (int i = 0; i < distance; i++)
        {
            environment = environment?.Enclosing;
        }

        if (environment is null) throw new RuntimeException("Can't lookup variable.");
        return environment;
    }
}