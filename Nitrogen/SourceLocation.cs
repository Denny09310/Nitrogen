namespace Nitrogen;

public readonly struct SourceLocation(int line, int column)
{
    public int Column { get; } = column;
    public int Line { get; } = line;

    public SourceSpan AsSpan(SourceLocation other)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(Column, other.Column);
        return new SourceSpan(this, other);
    }
}
