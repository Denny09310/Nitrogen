using Nitrogen.Core;
using Nitrogen.Core.Exceptions;
using Nitrogen.Core.Interpreting;
using Nitrogen.Core.Interpreting.Declarations;
using Nitrogen.Core.Syntax.Expressions.Abstractions;
using Nitrogen.Core.Syntax.Statements.Abstractions;
using Nitrogen.Core.Utils;

namespace Nitrogen.Interpreting;

public partial class Interpreter : IInterpreter
{
    private readonly Environment _globals;
    private readonly ModuleLoader _loader;
    private readonly Dictionary<IExpression, int> _locals = [];
    private readonly InterpreterOptions _options = InterpreterOptions.Default;

    private IEnvironment _environment;

    public Interpreter() : this(InterpreterOptions.Default)
    {
    }

    public Interpreter(InterpreterOptions options)
    {
        _globals = InitializeGlobals();
        _environment = new Environment(_globals);
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _loader = new ModuleLoader(Directory.GetCurrentDirectory());
    }

    public IEnvironment Environment => _environment;
    public Dictionary<IExpression, int> Locals => _locals;
    public IOutputSink Output => _options.OutputSink;

    public void Execute(List<IStatement> statements)
    {
        foreach (var statement in statements)
        {
            Execute(statement);
        }
    }

    public void Execute(List<IStatement> statements, IEnvironment environment)
    {
        (var enclosing, _environment) = (_environment, environment);

        try
        {
            foreach (var statement in statements)
            {
                Execute(statement);
            }
        }
        finally
        {
            _environment = enclosing;
        }
    }

    public void Execute(IStatement statement, IEnvironment environment)
    {
        (var enclosing, _environment) = (_environment, environment);

        try
        {
            Execute(statement);
        }
        finally
        {
            _environment = enclosing;
        }
    }

    public void Resolve(IExpression expression, int depth)
    {
        _locals.TryAdd(expression, depth);
    }

    private static Environment InitializeGlobals()
    {
        var environment = new Environment();

        var classes = TypeLoader.FindClasses(typeof(Interpreter));

        foreach (var @class in classes)
        {
            if (Activator.CreateInstance(@class) is not NativeInstance instance)
            {
                throw new RuntimeException($"Type '{@class.Name}' can't be instantiated into 'global scope'");
            }

            environment.Define(instance.Name, instance);
        }

        var functions = TypeLoader.FindFunctions(typeof(Interpreter));

        foreach (var function in functions)
        {
            if (Activator.CreateInstance(function) is not ICallable instance)
            {
                throw new RuntimeException($"Type '{function.Name}' can't be instantiated into 'global scope'");
            }

            environment.Define(instance.Name, instance);
        }

        return environment;
    }

    private object? LookupVariable(IExpression expression, Token name, bool global = true)
    {
        if (_locals.TryGetValue(expression, out var distance))
        {
            return _environment.GetAt(name, distance);
        }

        if (!global)
        {
            throw new RuntimeException(name, $"Global lookup not available for '{name.Lexeme}'");
        }

        return _globals.Get(name);
    }

    private void Loop(Func<bool> condition, IStatement body, IExpression? increment = null)
    {
        while (condition())
        {
            try
            {
                Execute(body);
            }
            catch (BreakException)
            {
                break;
            }
            catch (ContinueException)
            {
                continue;
            }
            finally
            {
                if (increment != null)
                {
                    Evaluate(increment);
                }
            }
        }
    }
}