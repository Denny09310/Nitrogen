using Nitrogen.Exceptions;
using Nitrogen.Interpreting;
using Nitrogen.Interpreting.Binding;
using Nitrogen.Lexing;
using Nitrogen.Parsing;
using Nitrogen.Syntax;
using Nitrogen.Syntax.Abstractions;

namespace Nitrogen.Runtime;

internal static class Program
{
    private static readonly Interpreter _interpreter = new();
    private static readonly AbstractSyntaxTree _sintaxTree = new();

    public static bool HasRuntimeErrors { get; set; }
    public static bool IsInteractive { get; set; }
    public static bool ShowAbstractSyntaxTree { get; set; }

    private static void CaptureErrors(List<SyntaxException> errors)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        foreach (var error in errors)
        {
            Console.WriteLine($"{error.Message} Line {error.Location.Line} Col {error.Location.Column}");
        }

        Console.WriteLine();
        Console.ResetColor();
    }

    private static void CaptureErrors(List<ParseException> errors)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        foreach (var error in errors)
        {
            var location = error.Token.Span.Start;
            Console.WriteLine($"{error.Message} Line {location.Line} Col {location.Column}");
        }

        Console.WriteLine();
        Console.ResetColor();
    }

    private static void CaptureErrors(RuntimeException ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(ex.Message);
        Console.WriteLine();
        Console.ResetColor();

        HasRuntimeErrors = true;
    }

    private static void CaptureErrors(List<BindingException> errors)
    {
        foreach (var error in errors)
        {
            Console.ForegroundColor = error.Level switch
            {
                ExceptionLevel.Info => ConsoleColor.Cyan,
                ExceptionLevel.Warning => ConsoleColor.Yellow,
                ExceptionLevel.Error => ConsoleColor.Red,
                _ => ConsoleColor.White,
            };

            var location = error.Token.Span.Start;
            Console.WriteLine($"{error.Message} Line {location.Line} Col {location.Column}");
        }

        Console.WriteLine();
        Console.ResetColor();
    }

    private static void Main(string[] args)
    {
        if (args is [var path])
        {
            RunFile(path);
        }
        else
        {
            RunInteractive();
        }
    }

    private static void Run(string source)
    {
        var tokens = RunLexer(source);

        if (tokens.Count == 0) return;
        var statements = RunParser(tokens);

        if (statements.Count == 0) return;

        RunResolver(statements);
    }

    private static void RunFile(string path)
    {
        var source = File.ReadAllText(path);
        Run(source);
    }

    static void RunInteractive()
    {
        const string Prompt = "> ";
        const string ExitCommand = "exit";
        const string ClearCommand = "clear";
        const string ShowAbstractSyntaxTreeCommand = "show-ast";

        IsInteractive = true;

        while (true)
        {
            Console.Write(Prompt);

            if (Console.ReadLine() is not string source) continue;

            if (source == ExitCommand)
            {
                break;
            }
            else if (source == ClearCommand)
            {
                Console.Clear();
                continue;
            }
            else if (source == ShowAbstractSyntaxTreeCommand)
            {
                ShowAbstractSyntaxTree = !ShowAbstractSyntaxTree;
                Console.WriteLine($"{(ShowAbstractSyntaxTree ? "Showing" : "Hiding")} Abstract Syntax Tree");
                continue;
            }

            Run(source);
        }

        IsInteractive = false;
    }

    private static void RunInterpreter(List<IStatement> statements)
    {
        if (ShowAbstractSyntaxTree)
        {
            Console.WriteLine(_sintaxTree.Print(statements));
        }

        try
        {
            _interpreter.Execute(statements);
        }
        catch (RuntimeException ex)
        {
            CaptureErrors(ex);
        }
    }

    private static List<Token> RunLexer(string source)
    {
        var lexer = Lexer.FromSource(source);
        var (tokens, errors) = lexer.Tokenize();

        if (errors.Count > 0)
        {
            CaptureErrors(errors);
            return [];
        }

        return tokens;
    }

    private static List<IStatement> RunParser(List<Token> tokens)
    {
        var parser = new Parser(tokens);
        var (statements, errors) = parser.Parse();

        if (errors.Count > 0)
        {
            CaptureErrors(errors);
            return [];
        }

        return statements;
    }

    private static void RunResolver(List<IStatement> statements)
    {
        var resolver = new Resolver(_interpreter);
        var errors = resolver.Resolve(statements);

        bool hasErrors = IsInteractive
            ? errors.Exists(error => error.Level is ExceptionLevel.Error)
            : errors.Count > 0;

        if (hasErrors)
        {
            CaptureErrors(errors);
            return;
        }

        RunInterpreter(statements);
    }
}