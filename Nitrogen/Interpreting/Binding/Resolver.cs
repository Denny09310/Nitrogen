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
    private readonly Stack<(Dictionary<int, Variable> variables, HashSet<int> variableHashes)> _scopes = [];

    private ClassType _currentClass;
    private FunctionType _currentFunction;
    private Loop? _currentLoop;

    public void BeginScope() => _scopes.Push((new Dictionary<int, Variable>(), new HashSet<int>()));

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

    private static bool HasReturn(IStatement statement)
    {
        if (statement is ReturnExpression) return true;

        if (statement is IfStatement ifStmt)
        {
            return HasReturn(ifStmt.Then) && ifStmt.Else != null && HasReturn(ifStmt.Else);
        }
        return false;
    }

    private void AddVariable(string name, Variable? variable = null)
    {
        variable ??= new Variable { Declared = true, Defined = true, Used = true, Name = new Token { Lexeme = name } };

        var (variables, variableHashes) = _scopes.Peek();
        int hash = name.GetHashCode();

        variables[hash] = variable;
        variableHashes.Add(hash);
    }

    private void Declare(Token name)
    {
        if (_scopes.Count == 0)
        {
            Report(ExceptionLevel.Error, name, "No scopes available.");
            return;
        }

        var (variables, variableHashes) = _scopes.Peek();
        int hash = name.Lexeme.GetHashCode();

        // Check if the variable already exists in the current scope
        if (!variableHashes.Contains(hash))
        {
            Report(ExceptionLevel.Error, name, $"Variable '{name.Lexeme}' not defined.");
            return;
        }

        if (variables.TryGetValue(hash, out var variable))
        {
            variable.Declared = true;
        }
    }

    private void Define(Token name)
    {
        if (_scopes.Count == 0)
        {
            Report(ExceptionLevel.Error, name, "No scopes available.");
            return;
        }

        var (variables, variableHashes) = _scopes.Peek();
        int hash = name.Lexeme.GetHashCode();

        // Attempt to add the variable to the current scope
        if (!variableHashes.Add(hash))
        {
            Report(ExceptionLevel.Error, name, $"Variable with name '{name.Lexeme}' already declared in this scope.");
            return;
        }

        variables[hash] = new Variable { Name = name, Defined = true };
    }

    private void EndScope()
    {
        var (variables, _) = _scopes.Pop();
        foreach (var variable in variables.Values)
        {
            if (!variable.Used)
            {
                Report(ExceptionLevel.Warning, variable.Name, $"Unused variable '{variable.Name.Lexeme}'.");
            }
        }
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

        if (_currentFunction != FunctionType.Constructor && !HasReturn(statement.Body))
        {
            Report(ExceptionLevel.Error, statement.Name, "All paths must return a value in non-void function.");
        }

        EndScope();

        _currentFunction = enclosing;
    }

    private void ResolveLocal(IExpression expression, Token name)
    {
        int hash = name.Lexeme.GetHashCode();

        foreach (var (variables, variableHashes) in _scopes)
        {
            if (variableHashes.Contains(hash) && variables.TryGetValue(hash, out var variable))
            {
                variable.Used = true;
                interpreter.Resolve(expression, _scopes.Count - 1);
                return;
            }
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
        public Token Name { get; set; }
        public bool Used { get; set; }
    }
}

