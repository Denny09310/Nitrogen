using System.Text;
using Nitrogen.Core;
using Nitrogen.Core.Exceptions;
using Nitrogen.Core.Syntax.Statements.Abstractions;
using Nitrogen.Interpreting;
using Nitrogen.Interpreting.Binding;
using Nitrogen.Lexing;
using Nitrogen.Parsing;

namespace Nitrogen.Runtime;

public static class Program
{
    private static readonly Interpreter _interpreter = new();
    private static readonly Resolver _resolver = new(_interpreter);

    public static bool HasRuntimeErrors { get; set; }
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
            Console.WriteLine($"{error.Message}");
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

    private static void RunInteractive()
    {
        const string ExitCommand = "exit";
        const string ClearCommand = "clear";

        StringBuilder sb = new();
        int count = 0; // Counter for consecutive Enter presses

        while (true)
        {
            if (Console.ReadLine() is not string input)
            {
                continue;
            }

            if (input == ExitCommand)
            {
                break;
            }
            else if (input == ClearCommand)
            {
                Console.Clear();
                continue;
            }
            else if (string.IsNullOrWhiteSpace(input))
            {
                // Increment the Enter key press count if input is empty (Enter pressed)
                count++;
            }
            else
            {
                // Reset the Enter key press count if any other input is received
                count = 0;

                // Append the command to the StringBuilder
                sb.AppendLine(input);
            }

            // Execute if Enter is pressed twice
            if (count == 1)
            {
                Run(sb.ToString());

                // Reset buffer and counter
                sb.Clear();
                count = 0;
            }
        }
    }

    private static void RunInterpreter(List<IStatement> statements)
    {
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
        var errors = _resolver.Resolve(statements);

        if (errors.Count > 0)
        {
            CaptureErrors(errors);
            return;
        }

        RunInterpreter(statements);
    }
}