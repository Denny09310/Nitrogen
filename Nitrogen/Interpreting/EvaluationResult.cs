namespace Nitrogen.Interpreting;

internal readonly struct EvaluationResult(object? value)
{
    public object? Value { get; } = value;

    public static object? operator -(EvaluationResult left, EvaluationResult right) => (left.Value, right.Value) switch
    {
        (double string1, double string2) => string1 - string2,
        _ => throw new InvalidOperationException($"Unsupported operation between types {left.Value?.GetType()} and {right.Value?.GetType()}."),
    };

    public static object? operator *(EvaluationResult left, EvaluationResult right) => (left.Value, right.Value) switch
    {
        (double string1, double string2) => string1 * string2,
        _ => throw new InvalidOperationException($"Unsupported operation between types {left.Value?.GetType()} and {right.Value?.GetType()}."),
    };

    public static object? operator /(EvaluationResult left, EvaluationResult right) => (left.Value, right.Value) switch
    {
        (double string1, double string2) => string1 / string2,
        _ => throw new InvalidOperationException($"Unsupported operation between types {left.Value?.GetType()} and {right.Value?.GetType()}."),
    };

    public static object? operator +(EvaluationResult left, EvaluationResult right) => (left.Value, right.Value) switch
    {
        (string string1, string string2) => string1 + string2,
        (double string1, double string2) => string1 + string2,
        _ => throw new InvalidOperationException($"Unsupported operation between types {left.Value?.GetType()} and {right.Value?.GetType()}."),
    };
}