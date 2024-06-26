using Nitrogen.Syntax;
using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Interpreting;

internal partial class Interpreter
{
    private readonly RuntimeEnvironment _globals;
    private readonly Dictionary<IExpression, int> _locals = [];

    private RuntimeEnvironment _environment;

    public Interpreter()
    {
        _globals = DefineGlobals();
        _environment = new RuntimeEnvironment(_globals);
    }

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

    private static RuntimeEnvironment DefineGlobals()
    {
        var environment = new RuntimeEnvironment();
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