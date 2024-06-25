using Nitrogen.Exceptions;
using Nitrogen.Syntax;
using Nitrogen.Syntax.Abstractions;
using Nitrogen.Syntax.Statements;

namespace Nitrogen.Interpreting.Binding;

internal partial class Resolver(Interpreter interpreter)
{
    private readonly List<BindingException> _errors = [];
    private readonly Stack<Dictionary<string, Variable>> _scopes = [];

    private FunctionType _currentFunction;

    public List<BindingException> Resolve(List<IStatement> statements)
    {
        BeginScope();

        foreach (var statement in statements)
        {
            Resolve(statement);
        }

        EndScope();

        return _errors;
    }

    private void BeginScope() => _scopes.Push([]);

    private void Declare(Token name)
    {
        if (_scopes.Count == 0)
        {
            _errors.Add(new(ExceptionLevel.Error, name, "No scopes available."));
            return;
        }

        Variable? variable = null;
        foreach (var scope in _scopes)
        {
            if (scope.TryGetValue(name.Lexeme, out variable)) break;
        }

        if (variable == null)
        {
            _errors.Add(new(ExceptionLevel.Error, name, $"Variable '{name.Lexeme}' not declared."));
            return;
        }

        variable.Declared = true;
    }

    private void Define(Token name)
    {
        if (_scopes.Count == 0)
        {
            _errors.Add(new(ExceptionLevel.Error, name, "No scopes available."));
            return;
        }

        var scope = _scopes.Peek();
        scope.Add(name.Lexeme, new Variable { Name = name, Defined = true });
    }

    private void EndScope()
    {
        var scope = _scopes.Pop();

        foreach (var (name, variable) in scope)
        {
            if (!variable.Used)
            {
                _errors.Add(new(ExceptionLevel.Warning, variable.Name, $"Unusued variable '{name}'."));
            }
        }
    }

    private void ResolveLocal(IExpression statement, Token name)
    {
        for (int i = 0; i < _scopes.Count; i++)
        {
            var scope = _scopes.ElementAt(i);
            if (scope.TryGetValue(name.Lexeme, out var variable))
            {
                variable.Used = true;
                interpreter.Resolve(statement, i);
            }
        }
    }

    private sealed class Variable
    {
        public bool Declared { get; set; }
        public bool Defined { get; set; }
        public Token Name { get; init; }
        public bool Used { get; set; }
    }
}