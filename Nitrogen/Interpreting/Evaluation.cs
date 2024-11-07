using Nitrogen.Abstractions.Exceptions;
using Nitrogen.Abstractions.Extensions;
using System.Globalization;

namespace Nitrogen.Interpreting;

public readonly struct Evaluation(object? value)
{
    public static readonly Evaluation One = new((double)1);

    public object? Value { get; } = value.Unwrap();

    public static implicit operator bool(Evaluation evaluation)
    {
        if (evaluation.Value is null) return false;
        if (evaluation.Value is bool bool1) return bool1;
        return true;
    }

    #region Additive

    public static object? operator -(Evaluation left, Evaluation right) => (left.Value, right.Value) switch
    {
        (double double1, double double2) => double1 - double2,
        _ => throw new RuntimeException($"Unsupported operation between types {left.Value?.GetType()} and {right.Value?.GetType()}."),
    };

    public static object? operator +(Evaluation left, Evaluation right) => (left.Value, right.Value) switch
    {
        (string string1, string string2) => string1 + string2,
        (double double1, double double2) => double1 + double2,

        (string string1, _) => string1 + right.Value?.ToString(),
        (_, string string2) => left.Value?.ToString() + string2,

        _ => throw new RuntimeException($"Unsupported operation between types {left.Value?.GetType()} and {right.Value?.GetType()}."),
    };

    #endregion Additive

    #region Multiplicative

    public static object? operator *(Evaluation left, Evaluation right) => (left.Value, right.Value) switch
    {
        (double double1, double double2) => double1 * double2,
        _ => throw new RuntimeException($"Unsupported operation between types {left.Value?.GetType()} and {right.Value?.GetType()}."),
    };

    public static object? operator /(Evaluation left, Evaluation right) => (left.Value, right.Value) switch
    {
        (double double1, double double2) => double1 / double2,
        _ => throw new RuntimeException($"Unsupported operation between types {left.Value?.GetType()} and {right.Value?.GetType()}."),
    };

    #endregion Multiplicative

    #region Equality

    public static bool operator !=(Evaluation left, Evaluation right) => (left.Value, right.Value) switch
    {
        (string string1, string string2) => string1 != string2,
        (double double1, double double2) => !double1.Equals(double2),
        _ => false,
    };

    public static bool operator ==(Evaluation left, Evaluation right) => (left.Value, right.Value) switch
    {
        (string string1, string string2) => string1 == string2,
        (double double1, double double2) => double1.Equals(double2),
        _ => false,
    };

    #endregion Equality

    #region Comparison

    public static bool operator <(Evaluation left, Evaluation right) => (left.Value, right.Value) switch
    {
        (string string1, string string2) => string1.CompareTo(string2) < 0,
        (double double1, double double2) => double1 < double2,
        _ => false,
    };

    public static bool operator <=(Evaluation left, Evaluation right) => (left.Value, right.Value) switch
    {
        (string string1, string string2) => string1.CompareTo(string2) <= 0,
        (double double1, double double2) => double1 <= double2,
        _ => false,
    };

    public static bool operator >(Evaluation left, Evaluation right) => (left.Value, right.Value) switch
    {
        (string string1, string string2) => string1.CompareTo(string2) > 0,
        (double double1, double double2) => double1 > double2,
        _ => false,
    };

    public static bool operator >=(Evaluation left, Evaluation right) => (left.Value, right.Value) switch
    {
        (string string1, string string2) => string1.CompareTo(string2) >= 0,
        (double double1, double double2) => double1 >= double2,
        _ => false,
    };

    #endregion Comparison

    #region Unary

    public static object? operator -(Evaluation left) => left.Value switch
    {
        (double double1) => -double1,
        _ => throw new RuntimeException($"Unsupported operation for type {left.Value?.GetType()}."),
    };

    #endregion Unary

    public override bool Equals(object? obj)
    {
        if (obj is not Evaluation result) return false;
        return this == result;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Value);
    }

    public override string? ToString()
    {
        if (Value == null) return "nil";
        if (Value is double double1) return double1.ToString(CultureInfo.InvariantCulture);
        return Value.ToString();
    }
}