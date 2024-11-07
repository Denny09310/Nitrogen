﻿using Nitrogen.Abstractions;
using Nitrogen.Abstractions.Exceptions;
using Nitrogen.Abstractions.Interpreting;
using System.Collections;

namespace Nitrogen.Interpreting;

public class Environment : IEnvironment
{
    private readonly Dictionary<string, object?> _variables = [];

    public Environment()
    {
    }

    public Environment(IEnvironment enclosing)
    {
        Enclosing = enclosing;
    }

    public IEnvironment? Enclosing { get; private set; }

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

    public void AssignAt(int distance, Token name, object? value)
    {
        Ancestor(distance).Assign(name, value);
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

    public object? GetAt(string name, int distance)
    {
        return Ancestor(distance).Get(name);
    }

    private IEnvironment Ancestor(int distance)
    {
        IEnvironment environment = this;
        for (int i = 0; i < distance; i++)
        {
            environment = environment.Enclosing ?? throw new RuntimeException("Can't lookup variable.");
        }
        return environment;
    }

    public IEnumerator<string> GetEnumerator()
    {
        return _variables.Keys.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
