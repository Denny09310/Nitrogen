using Nitrogen.Exceptions;
using Nitrogen.Syntax.Expressions;
using Nitrogen.Syntax.Statements;

namespace Nitrogen.Interpreting.Declarations;

public class FunctionDeclaration(FunctionStatement statement, Environment closure, bool isConstructor = false) : ICallable
{
    public Environment Closure { get; } = closure;

    public string Name { get; } = statement.Name.Lexeme;

    public void EnsureArity(object?[] @params)
    {
        var mandatory = statement.Arguments.Where(argument => argument is IdentifierExpression).ToArray();
        var optionals = statement.Arguments.Where(argument => argument is AssignmentExpression).ToArray();

        if (@params.Length < mandatory.Length || @params.Length > mandatory.Length + optionals.Length)
        {
            throw new RuntimeException(statement.Name, $"Parameters mismatch, required at least {mandatory.Length} and at most {mandatory.Length + optionals.Length}");
        }
    }

    public FunctionDeclaration Bind(ClassInstance instance)
    {
        var environment = new Environment(Closure);
        environment.Define("this", instance);
        return new FunctionDeclaration(statement, environment);
    }

    public object? Call(Interpreter interpreter, object?[] @params)
    {
        var environment = new Environment(Closure);
        DefineArguments(interpreter, @params, environment);

        try
        {
            interpreter.ExecuteScoped(statement.Body is BlockStatement block ? block.Statements : [statement.Body], new Environment(environment));
        }
        catch (ReturnException ex)
        {
            if (ex.Value is not null)
            {
                return ex.Value;
            }
        }

        return isConstructor ? Closure.Get("this") : null;
    }

    public override string ToString() => $"function {Name}() {{...}}";

    private void DefineArguments(Interpreter interpreter, object?[] @params, Environment environment)
    {
        foreach (var (index, argument) in statement.Arguments.Select((a, i) => (i, a)))
        {
            if (argument is IdentifierExpression identifier)
            {
                environment.Define(identifier.Name, @params[index]);
            }
            else if (argument is AssignmentExpression assignment)
            {
                if (index < @params.Length)
                {
                    environment.Define(assignment.Name, @params[index]);
                }
                else
                {
                    environment.Define(assignment.Name, interpreter.Evaluate(assignment.Value));
                }
            }
        }
    }
}