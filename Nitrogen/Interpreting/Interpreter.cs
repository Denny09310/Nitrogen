using Nitrogen.Syntax;

namespace Nitrogen.Interpreting;

internal partial class Interpreter
{
    private readonly RuntimeEnvironment _globals = DefineGlobals();
    private readonly Dictionary<Token, int> _locals = [];

    private RuntimeEnvironment _environment = new();

    public void Resolve(Token name, int depth) => _locals.Add(name, depth);

    private static RuntimeEnvironment DefineGlobals()
    {
        return new RuntimeEnvironment();
    }

    private object? LookupVariable(Token name)
    {
        if (_locals.TryGetValue(name, out var distance))
        {
            return _environment.GetVariableAt(name, distance);
        }

        return _globals.GetVariable(name);
    }
}