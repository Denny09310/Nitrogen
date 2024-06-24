using Nitrogen.Interpreting;
using Nitrogen.Lexing;
using Nitrogen.Parsing;

namespace Nitrogen.Runtime;

internal static class Program
{
    private static readonly Interpreter _interpreter = new();
    private static readonly AbstractSyntaxTree _sintaxTree = new();

    public static bool ShowAbstractSyntaxTree { get; set; }

    private static void Main(string[] args)
    {
        if (args is [var _])
        {
            // TODO: Read the file and send the content to the lexer
        }
        else
        {
            RunInteractive();
        }
    }

    static void Run(string source)
    {
        var lexer = Lexer.FromSource(source);
        var tokens = lexer.Tokenize();

        var parser = new Parser(tokens);
        var expressions = parser.Parse();

        if (ShowAbstractSyntaxTree)
        {
            Console.WriteLine(_sintaxTree.Print(expressions));
            return;
        }

        _interpreter.Execute(expressions);
    }

    static void RunInteractive()
    {
        const string Prompt = "> ";
        const string ExitCommand = "exit";
        const string ClearCommand = "clear";
        const string ShowAbstractSyntaxTreeCommand = "show-ast";

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
    }
}