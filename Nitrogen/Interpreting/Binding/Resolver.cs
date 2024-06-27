using Nitrogen.Exceptions;
using Nitrogen.Syntax;
using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Interpreting.Binding;

internal partial class Resolver
{
    #region Modules

    private readonly FunctionResolver _functions;
    private readonly Interpreter _interpreter;
    private readonly LoopResolver _loops;

    #endregion Modules

    private readonly List<BindingException> _errors = [];
    private readonly Dictionary<int, Variable> _variables = [];

    private int _currentDepth;

    public Resolver(Interpreter interpreter)
    {
        _interpreter = interpreter;
        _functions = new(this);
        _loops = new(this);
    }

    public void BeginScope() => _currentDepth++;

    public void Declare(Token name)
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

    public void Define(Token name)
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

    public void EndScope()
    {
        foreach (var variable in _variables.Values.Where(variable => variable.Depth == _currentDepth))
        {
            if (!variable.Used)
            {
                _errors.Add(new(ExceptionLevel.Warning, variable.Name, $"Unusued variable '{variable.Name.Lexeme}'."));
            }
        }

        _currentDepth--;
    }

    public void Error(ExceptionLevel level, Token token, string message) => _errors.Add(new(level, token, message));

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

    private void ResolveLocal(IExpression statement, Token name)
    {
        if (_variables.TryGetValue(name.Lexeme.GetHashCode(), out var variable))
        {
            variable.Used = true;
            _interpreter.Resolve(statement, _currentDepth - variable.Depth);
        }
        else
        {
            _interpreter.Resolve(statement, _currentDepth - 1);
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