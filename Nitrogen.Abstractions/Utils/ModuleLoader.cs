using Nitrogen.Abstractions.Interpreting;
using Nitrogen.Abstractions.Syntax.Expressions.Abstractions;

namespace Nitrogen.Abstractions.Utils;

public class ModuleLoader(string @base)
{
    private readonly string _base = @base;

    private readonly Dictionary<string, Module> _cache = [];

    public Module Load(string path, IModuleEvaluator evaluator)
    {
        // Step 1: Resolve the full path
        string fullPath = ResolvePath(path);
        if (_cache.TryGetValue(fullPath, out var cached))
        {
            // Return cached module if it's already loaded
            return cached;
        }

        // Step 2: Load and read the module file
        if (!Path.HasExtension(fullPath))
        {
            fullPath = Path.ChangeExtension(fullPath, "nt");
        }

        string content = File.ReadAllText(fullPath);

        // Step 3: Parse and evaluate the module content
        var module = evaluator.Evaluate(content);

        // Step 4: Cache the loaded module
        _cache[fullPath] = module;

        return module;
    }

    private string ResolvePath(string sourcePath)
    {
        if (Path.IsPathRooted(sourcePath))
        {
            return sourcePath;
        }

        // Ensure paths are relative to the base directory
        return Path.Combine(_base, sourcePath);
    }
}

public class Module(IEnvironment environment, IDictionary<IExpression, int> locals)
{
    public IEnvironment Environment { get; set; } = environment;
    public IDictionary<IExpression, int> Locals { get; } = locals;

    public static Module Create(IInterpreter interpreter) => new(interpreter.Environment, interpreter.Locals);
}