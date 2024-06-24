using System.Globalization;

namespace Nitrogen.Interpreting;

internal readonly struct EvaluationResult(object? value)
{
    public object? Value { get; } = value;

    public static implicit operator bool(EvaluationResult evaluation)
    {
        if (evaluation.Value is null) return false;
        if (evaluation.Value is bool bool1) return bool1;
        return true;
    }

    #region Additive

    public static object? operator -(EvaluationResult left, EvaluationResult right) => (left.Value, right.Value) switch
    {
        (double double1, double double2) => double1 - double2,
        _ => throw new InvalidOperationException($"Unsupported operation between types {left.Value?.GetType()} and {right.Value?.GetType()}."),
    };

    public static object? operator +(EvaluationResult left, EvaluationResult right) => (left.Value, right.Value) switch
    {
        (string string1, string string2) => string1 + string2,
        (double double1, double double2) => double1 + double2,
        _ => throw new InvalidOperationException($"Unsupported operation between types {left.Value?.GetType()} and {right.Value?.GetType()}."),
    };

    #endregion Additive

    #region Multiplicative

    public static object? operator *(EvaluationResult left, EvaluationResult right) => (left.Value, right.Value) switch
    {
        (double double1, double double2) => double1 * double2,
        _ => throw new InvalidOperationException($"Unsupported operation between types {left.Value?.GetType()} and {right.Value?.GetType()}."),
    };

    public static object? operator /(EvaluationResult left, EvaluationResult right) => (left.Value, right.Value) switch
    {
        (double double1, double double2) => double1 / double2,
        _ => throw new InvalidOperationException($"Unsupported operation between types {left.Value?.GetType()} and {right.Value?.GetType()}."),
    };

    #endregion Multiplicative

    #region Equality

    public static bool operator !=(EvaluationResult left, EvaluationResult right) => (left.Value, right.Value) switch
    {
        (string string1, string string2) => string1 != string2,
        (double double1, double double2) => !double1.Equals(double2),
        _ => false,
    };

    public static bool operator ==(EvaluationResult left, EvaluationResult right) => (left.Value, right.Value) switch
    {
        (string string1, string string2) => string1 == string2,
        (double double1, double double2) => double1.Equals(double2),
        _ => false,
    };

    #endregion Equality

    #region Comparison

    public static bool operator <(EvaluationResult left, EvaluationResult right) => (left.Value, right.Value) switch
    {
        (string string1, string string2) => string1.CompareTo(string2) < 0,
        (double double1, double double2) => double1 < double2,
        _ => false,
    };

    public static bool operator <=(EvaluationResult left, EvaluationResult right) => (left.Value, right.Value) switch
    {
        (string string1, string string2) => string1.CompareTo(string2) <= 0,
        (double double1, double double2) => double1 <= double2,
        _ => false,
    };

    public static bool operator >(EvaluationResult left, EvaluationResult right) => (left.Value, right.Value) switch
    {
        (string string1, string string2) => string1.CompareTo(string2) > 0,
        (double double1, double double2) => double1 > double2,
        _ => false,
    };

    public static bool operator >=(EvaluationResult left, EvaluationResult right) => (left.Value, right.Value) switch
    {
        (string string1, string string2) => string1.CompareTo(string2) >= 0,
        (double double1, double double2) => double1 >= double2,
        _ => false,
    };

    #endregion Comparison

    public override bool Equals(object? obj)
    {
        if (obj is not EvaluationResult result) return false;
        return this == result;
    }

    public override string? ToString()
    {
        if (Value == null) return "nil";
        if (Value is double double1) return double1.ToString(CultureInfo.InvariantCulture);
        return base.ToString();
    }
}