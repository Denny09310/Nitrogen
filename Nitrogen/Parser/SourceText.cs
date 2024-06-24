namespace Nitrogen.Parser;

internal class SourceText(string source)
{
    private readonly Lazy<string[]> _lines = new(() => source.Split('\n'));

    public string[] Lines => _lines.Value;

    public static SourceText FromSource(string source) => new(source);
}