namespace Nitrogen.Core.Interpreting;

public interface IEnvironment : IEnumerable<string>
{
    IEnvironment? Enclosing { get; }

    void Assign(Token name, object? value);
    void AssignAt(int distance, Token name, object? value);
    void Define(Token name, object? value);
    void Define(string name, object? value);
    object? Get(Token name);
    object? Get(string name);
    object? GetAt(Token name, int distance);
    object? GetAt(string name, int distance);
}
