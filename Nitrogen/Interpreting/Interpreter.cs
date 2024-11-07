using Nitrogen.Abstractions;
using Nitrogen.Abstractions.Base;
using Nitrogen.Abstractions.Declarations;
using Nitrogen.Abstractions.Exceptions;
using Nitrogen.Abstractions.Interpreting;
using Nitrogen.Abstractions.Syntax.Expressions.Abstractions;
using Nitrogen.Abstractions.Syntax.Statements.Abstractions;
using Nitrogen.Abstractions.Utils;

namespace Nitrogen.Interpreting;

public partial class Interpreter(InterpreterOptions options) : IInterpreter
{
    private readonly ModuleLoader _loader = new(Directory.GetCurrentDirectory());
    private readonly Dictionary<IExpression, int> _locals = [];
    private readonly InterpreterOptions _options = options ?? throw new ArgumentNullException(nameof(options));

    private IEnvironment _environment = InitializeGlobals();

    public Interpreter() : this(InterpreterOptions.Default)
    {
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
            if (Activator.CreateInstance(function) is not CallableBase instance)
            {
                throw new RuntimeException($"Type '{function.Name}' can't be instantiated into 'global scope'");
            }

            environment.Define(instance.Name, instance);
        }

        return environment;
    }

    private object? LookupVariable(IExpression expression, Token name)
    {
        if (_locals.TryGetValue(expression, out var distance))
        {
            return _environment.GetAt(name, distance);
        }

        throw new RuntimeException(name, $"Global lookup not available for '{name.Lexeme}'");
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