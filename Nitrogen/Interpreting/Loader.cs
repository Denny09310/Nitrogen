using Nitrogen.Exceptions;
using Nitrogen.Interpreting.Binding;
using Nitrogen.Lexing;
using Nitrogen.Parsing;
using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Interpreting;

public class Loader(string @base)
{
    private readonly string _base = @base;

    private readonly Dictionary<string, Module> _cache = [];

    public Module LoadModule(string sourcePath)
    {
        // Step 1: Resolve the full path
        string fullPath = ResolvePath(sourcePath);
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

        string moduleContent = File.ReadAllText(fullPath);

        // Step 3: Parse and evaluate the module content
        var module = ParseAndEvaluateModule(moduleContent);

        // Step 4: Cache the loaded module
        _cache[fullPath] = module;

        return module;
    }

    private static void EnsureSuccess(List<ParseException> errors)
    {
        if (errors.Count > 0)
        {
            var ex = errors[0];
            throw new RuntimeException(ex.Message, ex);
        }
    }

    private static void EnsureSuccess(List<SyntaxException> errors)
    {
        if (errors.Count > 0)
        {
            var ex = errors[0];
            throw new RuntimeException(ex.Message, ex);
        }
    }

    private static void EnsureSuccess(List<BindingException> errors)
    {
        if (errors.Count > 0)
        {
            var ex = errors[0];
            throw new RuntimeException(ex.Message, ex);
        }
    }

    private static Module ParseAndEvaluateModule(string content)
    {
        // Assuming `Interpreter` is the class responsible for evaluating scripts
        var interpreter = new Interpreter();

        var lexer = Lexer.FromSource(content);
        var (tokens, syntaxErrors) = lexer.Tokenize();

        EnsureSuccess(syntaxErrors);

        var parser = new Parser(tokens);
        var (statements, parseErrors) = parser.Parse();

        EnsureSuccess(parseErrors);

        var resolver = new Resolver(interpreter, module: true);
        var resolverErrors = resolver.Resolve(statements);

        EnsureSuccess(resolverErrors);

        // Run the module content through the interpreter to populate symbols
        interpreter.Execute(statements);

        // Collect defined symbols from the interpreter's scope (function, variables, etc.)
        return Module.Create(interpreter);
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

public class Module(Environment environment, IDictionary<IExpression, int> locals)
{
    public Environment Environment { get; set; } = environment;
    public IDictionary<IExpression, int> Locals { get; } = locals;

    public static Module Create(Interpreter interpreter) => new(interpreter.Environment, interpreter.Locals);
}