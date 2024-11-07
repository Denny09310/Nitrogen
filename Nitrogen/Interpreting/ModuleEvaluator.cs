using Nitrogen.Abstractions.Exceptions;
using Nitrogen.Abstractions.Interpreting;
using Nitrogen.Abstractions.Utils;
using Nitrogen.Interpreting.Binding;
using Nitrogen.Lexing;
using Nitrogen.Parsing;

namespace Nitrogen.Interpreting
{
    internal class ModuleEvaluator : IModuleEvaluator
    {
        public static readonly ModuleEvaluator Instance = new();

        public Module Evaluate(string content)
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
    }
}