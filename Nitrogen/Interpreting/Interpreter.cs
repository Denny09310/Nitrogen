using Nitrogen.Syntax;
using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Interpreting;

public partial class Interpreter
{
    private readonly InterpreterEnvironment _globals;
    private readonly Dictionary<IExpression, int> _locals = [];
    private readonly InterpreterOptions _options = InterpreterOptions.Default;

    private InterpreterEnvironment _environment;

    public Interpreter()
    {
        _globals = DefineGlobals();
        _environment = new InterpreterEnvironment(_globals);
    }

    public Interpreter(InterpreterOptions options) : this()
    {
        _options = options;
    }

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

    private static InterpreterEnvironment DefineGlobals()
    {
        var environment = new InterpreterEnvironment();
        return environment;
    }

    private object? LookupVariable(IExpression expression, Token name)
    {
        if (_locals.TryGetValue(expression, out var distance))
        {
            return _environment.GetAt(name, distance);
        }

        return _globals.Get(name);
    }
}