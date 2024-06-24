using Nitrogen.Lexing;

if (args is [var _])
{
    // TODO: Read the file and send the content to the lexer
}
else
{
    RunInteractive();
}

void RunInteractive()
{
    const string Prompt = "> ";
    const string ExitCommand = "exit";

    while (true)
    {
        Console.Write(Prompt);

        if (Console.ReadLine() is not string source) continue;
        if (source == ExitCommand) break;

        Run(source);
    }
}

void Run(string source)
{
    var lexer = Lexer.FromSource(source);
    var tokens = lexer.Tokenize();

    foreach (var token in tokens)
    {
        Console.WriteLine(token);
    }
}