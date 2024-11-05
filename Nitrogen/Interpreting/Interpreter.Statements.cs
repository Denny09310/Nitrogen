using Nitrogen.Exceptions;
using Nitrogen.Interpreting.Declarations;
using Nitrogen.Syntax.Abstractions;
using Nitrogen.Syntax.Expressions;
using Nitrogen.Syntax.Statements;

using System.Diagnostics;

namespace Nitrogen.Interpreting;

public partial class Interpreter
{
    public object? Execute(IStatement stmt) => stmt switch
    {
        ExpressionStatement statement => Execute(statement),
        PrintStatement statement => Execute(statement),
        WhileStatement statement => Execute(statement),
        ForStatement statement => Execute(statement),
        BlockStatement statement => Execute(statement),
        IfStatement statement => Execute(statement),
        FunctionStatement statement => Execute(statement),
        VarStatement statement => Execute(statement),
        ClassStatement statement => Execute(statement),
        ImportStatement statement => Execute(statement),
        _ => throw new UnreachableException($"Statement {stmt.GetType()} not recognized.")
    };

    public void ExecuteScoped(List<IStatement> statements, Environment environment)
    {
        (var enclosing, _environment) = (_environment, environment);

        try
        {
            foreach (var statement in statements)
            {
                Execute(statement);
            }
        }
        finally
        {
            _environment = enclosing;
        }
    }

    private object? Execute(ImportStatement statement)
    {
        // Step 1: Evaluate the source expression to get the module path
        var source = Evaluate(statement.Source) as string ?? throw new ArgumentException("Invalid import 'source'.");

        // Step 2: Load the module
        var module = _loader.LoadModule(source) ?? throw new RuntimeException($"Module '{source}' could not be loaded.");

        foreach (var (key, value) in module.Locals)
        {
            Locals.Add(key, value);
        }

        // Step 3: Import each symbol specified in 'Imports'
        foreach (var import in statement.Imports)
        {
            // Assuming import is an `IdentifierExpression`
            string importName = ((IdentifierExpression)import).Name.Lexeme;

            // Step 4: Find and add the symbol to the current scope
            try
            {
                var symbol =  module.Environment.Get(importName);
                
                // Add to the current execution context, assume `currentScope` exists
                Environment.Define(importName, symbol);
            }
            catch (Exception ex)
            {
                throw new RuntimeException($"Symbol '{importName}' not found in module '{source}'.", ex);
            }
        }

        return null;
    }

    private object? Execute(ExpressionStatement statement)
    {
        return Evaluate(statement.Expression);
    }

    private object? Execute(PrintStatement statement)
    {
        var value = new EvaluationResult(Evaluate(statement.Expression));
        Output.WriteLine(value.ToString());

        return null;
    }

    private object? Execute(WhileStatement statement)
    {
        Loop(() => new EvaluationResult(Evaluate(statement.Condition)), statement.Body);
        return null;
    }

    private object? Execute(ForStatement statement)
    {
        if (statement.Initialization is not null) Execute(statement.Initialization);
        Loop(() => new EvaluationResult(Evaluate(statement.Condition)), statement.Body, statement.Increment);
        return null;
    }

    private object? Execute(BlockStatement statement)
    {
        ExecuteScoped(statement.Statements, new Environment(_environment));
        return null;
    }

    private object? Execute(IfStatement statement)
    {
        var condition = new EvaluationResult(Evaluate(statement.Condition));
        if (condition) Execute(statement.Then);
        else if (statement.Else is not null) Execute(statement.Else);

        return null;
    }

    private FunctionDeclaration Execute(FunctionStatement statement)
    {
        var function = new FunctionDeclaration(statement, _environment);
        _environment.Define(statement.Name, function);
        return function;
    }

    private object? Execute(VarStatement statement)
    {
        object? value = null;
        if (statement.Initializer is not null) value = Evaluate(statement.Initializer);
        _environment.Define(statement.Name, value);

        return null;
    }

    private object? Execute(ClassStatement statement)
    {
        ClassDeclaration? superclass = null;
        if (statement.Superclass is not null)
        {
            superclass = Evaluate(statement.Superclass) as ClassDeclaration
                ?? throw new RuntimeException(statement.Superclass.Name, "Superclass must be a class.");
        }

        _environment.Define(statement.Name, null);

        if (superclass is not null)
        {
            _environment = new Environment(_environment);
            _environment.Define("super", superclass);
        }

        var methods = statement.Methods.ToDictionary(
            method => method.Name.Lexeme,
            method => new FunctionDeclaration(method, _environment, method.Name.Lexeme is "constructor"));

        var @class = new ClassDeclaration(statement, superclass, methods);

        if (superclass is not null)
        {
            _environment = _environment.Enclosing ?? throw new RuntimeException("Can't leave global scope.");
        }

        _environment.Assign(statement.Name, @class);

        return null;
    }
}