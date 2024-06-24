using Nitrogen.Parser;

if (args is [var path])
{
    var content = await File.ReadAllTextAsync(path);
    var source = SourceText.FromSource(content);

    // TODO: Send the source to the lexer
}
else
{
    // TODO: Start an interactive session in which the user can write
}