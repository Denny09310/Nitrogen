namespace Nitrogen;

public class SourceText(string source)
{
    private readonly Lazy<string[]> _lines = new(() => source.Split('\n'));

    public int Length { get; } = source.Length;
    public string[] Lines => _lines.Value;

    public char this[int index] => CharAt(index);

    public static SourceText FromSource(string source) => new(source);

    public char CharAt(int index)
    {
        if (index < 0 || index > source.Length - 1) return '\0';
        return source[index];
    }
}