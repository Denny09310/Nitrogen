using Nitrogen.Exceptions;
using Nitrogen.Interpreting.Declarations.Classes;
using Nitrogen.Syntax;
using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Interpreting;

public partial class Interpreter
{
    private readonly Environment _globals;
    private readonly Loader _loader;
    private readonly Dictionary<IExpression, int> _locals = [];
    private readonly InterpreterOptions _options = InterpreterOptions.Default;

    private Environment _environment;

    public Interpreter() : this(InterpreterOptions.Default)
    {
    }

    public Interpreter(InterpreterOptions options)
    {
        _globals = DefineGlobals();
        _environment = new Environment(_globals);
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _loader = new Loader(Directory.GetCurrentDirectory());
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

    private Environment DefineGlobals()
    {
        var environment = new Environment();

        environment.Define("console", new ConsoleInstance(this));

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