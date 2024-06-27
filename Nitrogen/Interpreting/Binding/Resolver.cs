using Nitrogen.Exceptions;
using Nitrogen.Syntax;
using Nitrogen.Syntax.Abstractions;
using Nitrogen.Syntax.Expressions;
using Nitrogen.Syntax.Statements;
using System.Diagnostics;

namespace Nitrogen.Interpreting.Binding;

internal partial class Resolver(Interpreter interpreter)
{
    private readonly List<BindingException> _errors = [];
    private readonly Dictionary<int, Variable> _variables = [];

    private ClassType _currentClass;
    private int _currentDepth;

    private FunctionType _currentFunction;
    private Loop? _currentLoop;

    public void BeginScope() => _currentDepth++;

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

    private void AddVariable(string name, Variable? variable = null)
    {
        variable ??= new()
        {
            Defined = true,
            Declared = true,
            Used = true,
            Depth = _currentDepth,
        };

        variable.Name = new Token { Lexeme = name };

        _variables.Add(name.GetHashCode(), variable);
    }

    private void Declare(Token name)
    {
        if (_currentDepth == 0)
        {
            _errors.Add(new(ExceptionLevel.Error, name, "No scopes available."));
            return;
        }

        if (!_variables.TryGetValue(name.Lexeme.GetHashCode(), out var variable))
        {
            _errors.Add(new(ExceptionLevel.Error, name, $"Variable '{name.Lexeme}' not defined."));
            return;
        }

        variable.Declared = true;
    }

    private void Define(Token name)
    {
        if (_currentDepth == 0)
        {
            _errors.Add(new(ExceptionLevel.Error, name, "No scopes available."));
            return;
        }

        if (!_variables.TryAdd(name.Lexeme.GetHashCode(), new Variable { Name = name, Defined = true, Depth = _currentDepth }))
        {
            _errors.Add(new(ExceptionLevel.Error, name, $"Variable with name '{name.Lexeme}' already declared in this scope."));
        }
    }

    private void EndScope()
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

    private void Report(ExceptionLevel level, Token token, string message) => _errors.Add(new(level, token, message));

    private void ResolveArguments(IList<IExpression> arguments)
    {
        foreach (var argument in arguments)
        {
            if (argument is AssignmentExpression assignment)
            {
                Define(assignment.Name);
                Declare(assignment.Name);
            }
            else if (argument is IdentifierExpression identifier)
            {
                Define(identifier.Name);
                Declare(identifier.Name);
            }
        }
    }

    private void ResolveFunction(FunctionStatement statement, FunctionType type)
    {
        var enclosing = _currentFunction;
        _currentFunction = type;

        BeginScope();

        ResolveArguments(statement.Arguments);
        Resolve(statement.Body);

        EndScope();

        _currentFunction = enclosing;
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

    private void ResolveLoop(IStatement statement, LoopType type)
    {
        var enclosing = _currentLoop;
        _currentLoop = new Loop(type);

        Token token;
        if (statement is WhileStatement @while)
        {
            token = @while.Keyword;

            Resolve(@while.Condition);
            Resolve(@while.Body);

            if (@while.Condition is not LiteralExpression expression || expression.Literal is not bool bool1 || !bool1)
            {
                _currentLoop.Infinite = false;
            }
        }
        else if (statement is ForStatement @for)
        {
            token = @for.Keyword;

            if (@for.Initialization is not null) Resolve(@for.Initialization);
            Resolve(@for.Condition);
            Resolve(@for.Body);

            if (@for.Increment is not null)
            {
                _currentLoop.Infinite = false;

                Resolve(@for.Increment);
            }
        }
        else
        {
            throw new UnreachableException("Non loop statement detected.");
        }

        if (_currentLoop.Infinite)
        {
            Report(ExceptionLevel.Warning, token, "Possible infinite loop statement detected.");
        }

        _currentLoop = enclosing;
    }

    private sealed class Loop(LoopType type)
    {
        public bool Infinite { get; set; } = true;
        public LoopType Type { get; init; } = type;
    }

    private sealed class Variable
    {
        public bool Declared { get; set; }
        public bool Defined { get; set; }
        public int Depth { get; set; }
        public Token Name { get; set; }
        public bool Used { get; set; }
    }

    private sealed class Loop
    {
        public LoopType Type { get; set; }
        public bool CanExit { get; set; }
    }
}