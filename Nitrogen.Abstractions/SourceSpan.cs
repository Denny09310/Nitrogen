namespace Nitrogen.Abstractions;

public record struct SourceSpan(SourceLocation Start, SourceLocation End)
{
    public readonly int Length => End.Column - Start.Column;
}