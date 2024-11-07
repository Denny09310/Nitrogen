using Nitrogen.Abstractions.Exceptions;
using Nitrogen.Abstractions.Interpreting;
using Nitrogen.Abstractions.Syntax.Expressions;
using Nitrogen.Abstractions.Syntax.Statements;

namespace Nitrogen.Interpreting.Declarations;

public class FunctionDeclaration(FunctionStatement statement, IEnvironment closure, bool isConstructor = false) : IFunctionDeclaration
{
    private readonly FunctionStatement _statement = statement;

    public IEnvironment Closure { get; } = closure;

    public string Name => _statement.Name.Lexeme;

    public void Arity(object?[] args)
    {
        var mandatory = _statement.Arguments.Where(argument => argument is IdentifierExpression).ToArray();
        var optionals = _statement.Arguments.Where(argument => argument is AssignmentExpression).ToArray();

        if (args.Length < mandatory.Length || args.Length > mandatory.Length + optionals.Length)
        {
            throw new RuntimeException(_statement.Name, $"Parameters mismatch, required at least {mandatory.Length} and at most {mandatory.Length + optionals.Length}");
        }
    }

    public IFunctionDeclaration Bind(IClassInstance instance)
    {
        var environment = new Environment(Closure);
        environment.Define("this", instance);
        return new FunctionDeclaration(_statement, environment);
    }

    public object? Call(IInterpreter interpreter, object?[] args)
    {
        var environment = new Environment(Closure);
        DefineArguments(interpreter, args, environment);

        try
        {
            interpreter.Execute(_statement.Body is BlockStatement block ? block.Statements : [_statement.Body], new Environment(environment));
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

    private void DefineArguments(IInterpreter interpreter, object?[] args, Environment environment)
    {
        foreach (var (index, argument) in _statement.Arguments.Select((a, i) => (i, a)))
        {
            if (argument is IdentifierExpression identifier)
            {
                environment.Define(identifier.Name, args[index]);
            }
            else if (argument is AssignmentExpression assignment)
            {
                if (index < args.Length)
                {
                    environment.Define(assignment.Name, args[index]);
                }
                else
                {
                    environment.Define(assignment.Name, interpreter.Evaluate(assignment.Value));
                }
            }
        }
    }
}