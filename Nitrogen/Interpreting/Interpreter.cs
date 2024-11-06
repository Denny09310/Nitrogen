using Nitrogen.Exceptions;
using Nitrogen.Interpreting.Declarations;
using Nitrogen.Syntax;
using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Interpreting;

public partial class Interpreter
{
    private readonly Environment _globals;
    private readonly ModuleLoader _loader;
    private readonly Dictionary<IExpression, int> _locals = [];
    private readonly InterpreterOptions _options = InterpreterOptions.Default;

    private Environment _environment;

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

    public Environment Environment => _environment;
    public Dictionary<IExpression, int> Locals => _locals;
    public IOutputSink Output => _options.OutputSink;

    public void Execute(List<IStatement> statements)
    {
        foreach (var statement in statements)
        {
            Execute(statement);
        }
    }

    public void Resolve(IExpression expression, int depth)
    {
        _locals.TryAdd(expression, depth);
    }

    private static Environment InitializeGlobals()
    {
        var environment = new Environment();

        var classes = typeof(Interpreter).Assembly
            .ExportedTypes
            .Where(IsGlobalClass);

        foreach (var @class in classes)
        {
            if (Activator.CreateInstance(@class) is not NativeInstance instance)
            {
                throw new RuntimeException($"Type '{@class.Name}' can't be instantiated into 'global scope'");
            }

            environment.Define(instance.Name, instance);
        }

        var functions = typeof(Interpreter).Assembly
            .ExportedTypes
            .Where(IsGlobalFunction);

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

    private static bool IsGlobalClass(Type type)
    {
        return !type.IsAbstract
            && type.Name != nameof(WrapperInstance)
            && typeof(NativeInstance).IsAssignableFrom(type);
    }

    private static bool IsGlobalFunction(Type type)
    {
        return !type.IsAbstract
            && type.Name != nameof(FunctionDeclaration)
            && type.Name != nameof(MethodCallable)
            && type.Name != nameof(PropertyCallable)
            && typeof(CallableBase).IsAssignableFrom(type);
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