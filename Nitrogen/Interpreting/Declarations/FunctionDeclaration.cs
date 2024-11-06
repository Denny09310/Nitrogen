using Nitrogen.Exceptions;
using Nitrogen.Syntax.Expressions;
using Nitrogen.Syntax.Statements;

namespace Nitrogen.Interpreting.Declarations;

public class FunctionDeclaration(FunctionStatement statement, Environment closure, bool isConstructor = false) : CallableBase
{
    private readonly FunctionStatement _statement = statement;

    public Environment Closure { get; } = closure;

    public override string Name => _statement.Name.Lexeme;

    public override void Arity(object?[] args)
    {
        var mandatory = _statement.Arguments.Where(argument => argument is IdentifierExpression).ToArray();
        var optionals = _statement.Arguments.Where(argument => argument is AssignmentExpression).ToArray();

        if (args.Length < mandatory.Length || args.Length > mandatory.Length + optionals.Length)
        {
            throw new RuntimeException(_statement.Name, $"Parameters mismatch, required at least {mandatory.Length} and at most {mandatory.Length + optionals.Length}");
        }
    }

    public FunctionDeclaration Bind(ClassInstance instance)
    {
        var environment = new Environment(Closure);
        environment.Define("this", instance);
        return new FunctionDeclaration(_statement, environment);
    }

    public override object? Call(Interpreter interpreter, object?[] args)
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

    private void DefineArguments(Interpreter interpreter, object?[] args, Environment environment)
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