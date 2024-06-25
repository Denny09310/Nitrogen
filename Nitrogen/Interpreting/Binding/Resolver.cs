using Nitrogen.Exceptions;
using Nitrogen.Syntax;
using Nitrogen.Syntax.Abstractions;
using Nitrogen.Syntax.Expressions;
using Nitrogen.Syntax.Statements;

namespace Nitrogen.Interpreting.Binding;

internal partial class Resolver(Interpreter interpreter)
{
    private readonly List<BindingException> _errors = [];
    private readonly Stack<Dictionary<string, Variable>> _scopes = [];

    private FunctionType _currentFunction;

    public List<BindingException> Resolve(List<IStatement> statements)
    {
        foreach (var statement in statements)
        {
            Resolve(statement);
        }

        return _errors;
    }

    private void BeginScope() => _scopes.Push([]);

    private void Declare(Token name)
    {
        if (_scopes.Count == 0) return;

        var scope = _scopes.Peek();
        if (!scope.TryAdd(name.Lexeme, new Variable { Name = name, Defined = true }))
        {
            _errors.Add(new(ExceptionLevel.Error, name, "Variable with this name already declared in this scope."));
        }
    }

    private void Define(Token name)
    {
        if (_scopes.Count == 0) return;

        var scope = _scopes.Peek();
        if (!scope.TryGetValue(name.Lexeme, out var variable))
        {
            _errors.Add(new(ExceptionLevel.Error, name, $"Variable '{name.Lexeme}' not declared."));
            return;
        }

        variable.Declared = true;
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

    private void ResolveFunction(FunctionStatement statement, FunctionType type)
    {
        (var enclosingFunction, _currentFunction) = (_currentFunction, type);

        BeginScope();

        foreach (var argument in statement.Arguments)
        {
            if (argument is AssignmentExpression assignment)
            {
                Declare(assignment.Name);
                Define(assignment.Name);
            }
            else if (argument is IdentifierExpression identifier)
            {
                Declare(identifier.Name);
                Define(identifier.Name);
            }
        }

        Resolve(statement.Body);

        EndScope();

        _currentFunction = enclosingFunction;
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
                break;
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