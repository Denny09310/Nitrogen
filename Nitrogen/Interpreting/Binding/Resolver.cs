using Nitrogen.Exceptions;
using Nitrogen.Syntax;
using Nitrogen.Syntax.Abstractions;
using Nitrogen.Syntax.Expressions;
using Nitrogen.Syntax.Statements;

namespace Nitrogen.Interpreting.Binding;

internal partial class Resolver(Interpreter interpreter)
{
    private readonly List<BindingException> _errors = [];
    private readonly Dictionary<int, Variable> _variables = [];

    private int _currentDepth = 1;
    private FunctionType _currentFunction;

    public List<BindingException> Resolve(List<IStatement> statements)
    {
        foreach (var statement in statements)
        {
            Resolve(statement);
        }

        return _errors;
    }

    private void BeginScope() => _currentDepth++;

    private void Declare(Token name)
    {
        if (_currentDepth == 0)
        {
            _errors.Add(new(ExceptionLevel.Error, name, "No scopes available."));
            return;
        }

        if (!_variables.TryAdd(name.Lexeme.GetHashCode(), new Variable { Name = name, Defined = true, Depth = _currentDepth }))
        {
            _errors.Add(new(ExceptionLevel.Error, name, "Variable with this name already declared in this scope."));
        }
    }

    private void Define(Token name)
    {
        if (_currentDepth == 0)
        {
            _errors.Add(new(ExceptionLevel.Error, name, "No scopes available."));
            return;
        }

        if (!_variables.TryGetValue(name.Lexeme.GetHashCode(), out var variable))
        {
            _errors.Add(new(ExceptionLevel.Error, name, $"Variable '{name.Lexeme}' not declared."));
            return;
        }

        variable.Declared = true;
    }

    private void EndScope()
    {
        _currentDepth--;

        foreach (var variable in _variables.Values.Where(variable => variable.Depth == _currentDepth))
        {
            if (!variable.Used)
            {
                _errors.Add(new(ExceptionLevel.Warning, variable.Name, $"Unusued variable '{variable.Name.Lexeme}'."));
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
        if (_variables.TryGetValue(name.Lexeme.GetHashCode(), out var variable))
        {
            variable.Used = true;
            interpreter.Resolve(statement, _currentDepth - variable.Depth);
        }
        else
        {
            interpreter.Resolve(statement, _currentDepth - 1);
        }
    }

    private sealed class Variable
    {
        public bool Declared { get; set; }
        public bool Defined { get; set; }
        public int Depth { get; set; }
        public Token Name { get; init; }
        public bool Used { get; set; }
    }
}