using Nitrogen.Core;
using Nitrogen.Core.Exceptions;
using Nitrogen.Core.Syntax.Expressions;
using Nitrogen.Core.Syntax.Expressions.Abstractions;
using Nitrogen.Core.Syntax.Statements;
using Nitrogen.Core.Syntax.Statements.Abstractions;
using System.Diagnostics;

namespace Nitrogen.Interpreting.Binding;

public partial class Resolver(Interpreter interpreter, bool module = false)
{
    private readonly Interpreter _interpreter = interpreter;
    private readonly bool _module = module;

    private readonly List<BindingException> _errors = [];
    private readonly Stack<Dictionary<int, Variable>> _scopes = [];

    private ClassType _currentClass;
    private FunctionType _currentFunction;
    private Loop? _currentLoop;

    public void BeginScope() => _scopes.Push([]);

    public List<BindingException> Resolve(List<IStatement> statements)
    {
        BeginScope();
        InitializeGlobalScope();

        foreach (var statement in statements)
        {
            Resolve(statement);
        }

        EndScope();

        return _errors;
    }

    private void Define(string name, Variable? variable = null)
    {
        variable ??= new()
        {
            Declared = true,
            Defined = true,
            Used = true,
        };

        variable.Name = new Token { Lexeme = name };

        var scope = _scopes.Peek();
        scope.Add(name.GetHashCode(), variable);
    }

    private void Define(Token name)
    {
        if (_scopes.Count == 0)
        {
            Report(ExceptionLevel.Error, name, "No scopes available.");
            return;
        }

        var scope = _scopes.Peek();
        if (!scope.TryGetValue(name.Lexeme.GetHashCode(), out var variable))
        {
            Report(ExceptionLevel.Error, name, $"Variable '{name.Lexeme}' not declared.");
            return;
        }

        variable.Defined = true;
    }

    private void Declare(Token name)
    {
        if (_scopes.Count == 0)
        {
            Report(ExceptionLevel.Error, name, "No scopes available.");
            return;
        }

        var scope = _scopes.Peek();
        if (!scope.TryAdd(name.Lexeme.GetHashCode(), new Variable { Name = name, Declared = true }))
        {
            Report(ExceptionLevel.Error, name, $"Variable with name '{name.Lexeme}' already declared in this scope.");
        }
    }

    private void EndScope()
    {
        var scope = _scopes.Pop();
        foreach (var variable in scope.Values)
        {
            if (!_module && !variable.Used)
            {
                Report(ExceptionLevel.Warning, variable.Name, $"Unusued variable '{variable.Name.Lexeme}'.");
            }
        }
    }

    private void InitializeGlobalScope()
    {
        var global = _interpreter.Environment.Enclosing ?? throw new RuntimeException("'global' scope not initialized.");

        foreach (var item in global)
        {
            Define(item);
        }
    }

    private void Report(ExceptionLevel level, Token token, string message) => _errors.Add(new(level, token, message));

    private void ResolveArguments(IList<IExpression> arguments)
    {
        foreach (var argument in arguments)
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
        foreach (var (index, scope) in _scopes.Select((s, i) => (i, s)))
        {
            if (scope.TryGetValue(name.Lexeme.GetHashCode(), out var variable))
            {
                variable.Used = true;
                _interpreter.Resolve(statement, index);
                return;
            }
        }

        _interpreter.Resolve(statement, _scopes.Count - 1);
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
        public bool Defined { get; set; }
        public bool Declared { get; set; }
        public Token Name { get; set; }
        public bool Used { get; set; }
    }
}