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

    public void AssignVariable(Token name, object? value)
    {
        if (!_variables.ContainsKey(name.Lexeme))
        {
            throw new RuntimeException(name, $"Variable with name '{name.Lexeme}' not defined in this scope.");
        }

        _variables[name.Lexeme] = value;
    }

    public void DefineVariable(Token name, object? value)
    {
        if (!_variables.TryAdd(name.Lexeme, value))
        {
            throw new RuntimeException(name, $"Variable with name '{name.Lexeme}' already defined in this scope.");
        }
    }

    public object? GetVariable(Token name)
    {
        if (_variables.TryGetValue(name.Lexeme, out var value)) return value;
        if (Enclosing is not null) return Enclosing.GetVariable(name);
        throw new RuntimeException(name, $"Variable with name '{name.Lexeme}' not defined in this scope.");
    }
}