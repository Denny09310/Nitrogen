using Nitrogen.Exceptions;
using Nitrogen.Syntax.Expressions;
using Nitrogen.Syntax.Statements;

namespace Nitrogen.Interpreting.Declarations;

internal class FunctionDeclaration(FunctionStatement statement, RuntimeEnvironment closure) : ICallable
{
    public RuntimeEnvironment Closure { get; } = closure;

    public string Name { get; } = statement.Name.Lexeme;

    public void Arity(object?[] @params)
    {
        var mandatory = statement.Arguments.Where(argument => argument is IdentifierExpression).ToArray();
        var optionals = statement.Arguments.Where(argument => argument is AssignmentExpression).ToArray();

        if (@params.Length < mandatory.Length || @params.Length > mandatory.Length + optionals.Length)
        {
            throw new RuntimeException(statement.Name, $"Parameters mismatch, required at least {mandatory.Length} and at most {mandatory.Length + optionals.Length}");
        }
    }

    public object? Call(Interpreter interpreter, object?[] @params)
    {
        var environment = new RuntimeEnvironment(Closure);
        DefineArguments(interpreter, @params, environment);

        interpreter.ExecuteScoped(statement.Body is BlockStatement block ? block.Statements : [statement.Body], environment);

        return null;
    }

    private void DefineArguments(Interpreter interpreter, object?[] @params, RuntimeEnvironment environment)
    {
        foreach (var (index, argument) in statement.Arguments.Select((a, i) => (i, a)))
        {
            if (argument is IdentifierExpression identifier)
            {
                environment.DefineVariable(identifier.Name, @params[index]);
            }
            else if (argument is AssignmentExpression assignment)
            {
                if (index < @params.Length)
                {
                    environment.DefineVariable(assignment.Target, @params[index]);
                }
                else
                {
                    environment.DefineVariable(assignment.Target, interpreter.Evaluate(assignment.Value));
                }
            }
        }
    }
}