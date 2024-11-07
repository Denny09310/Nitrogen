using Nitrogen.Core.Exceptions;
using Nitrogen.Core.Syntax.Expressions;
using Nitrogen.Core.Syntax.Statements;
using Nitrogen.Core.Syntax.Statements.Abstractions;
using System;

namespace Nitrogen.Interpreting.Binding;

public partial class Resolver
{
	public void Resolve(IStatement statement)
	{
		switch (statement)
		{
			case VarStatement variable: Resolve(variable); break;
			case ExpressionStatement expression: Resolve(expression); break;
			case WhileStatement @while: Resolve(@while); break;
			case ForStatement @for: Resolve(@for); break;
			case BlockStatement block: Resolve(block); break;
			case IfStatement @if: Resolve(@if); break;
			case FunctionStatement function: Resolve(function); break;
			case ClassStatement function: Resolve(function); break;
			case ImportStatement import: Resolve(import); break;

			default: break;
		}
	}

	private void Resolve(VarStatement statement)
	{
		Define(statement.Name);
		if (statement.Initializer is not null)
		{
			Resolve(statement.Initializer);
		}
		Declare(statement.Name);
	}

	private void Resolve(ExpressionStatement statement)
	{
		Resolve(statement.Expression);
	}

	private void Resolve(WhileStatement statement)
	{
		ResolveLoop(statement, LoopType.While);
	}

	private void Resolve(ForStatement statement)
	{
		ResolveLoop(statement, LoopType.For);
	}

	private void Resolve(BlockStatement statement)
	{
		BeginScope();
		foreach (var stmt in statement.Statements)
		{
			Resolve(stmt);
		}
		EndScope();
	}

	private void Resolve(IfStatement statement)
	{
		Resolve(statement.Condition);
		Resolve(statement.Then);
		if (statement.Else is not null) Resolve(statement.Else);
	}

	private void Resolve(FunctionStatement statement)
	{
		Define(statement.Name);
		Declare(statement.Name);

		ResolveFunction(statement, FunctionType.Function);
	}

	private void Resolve(ClassStatement statement)
	{
		var enclosing = _currentClass;
		_currentClass = ClassType.Class;

		Define(statement.Name);
		Declare(statement.Name);

		if (statement.Superclass is not null)
		{
			if (statement.Name.Lexeme == statement.Superclass.Name.Lexeme)
			{
				Report(ExceptionLevel.Error, statement.Superclass.Name, "A class can't inherit from itself.");
			}

			_currentClass = ClassType.Subclass;
			Resolve(statement.Superclass);

			BeginScope();
			Declare("super");
		}

		BeginScope();

		Declare("this");

		foreach (var method in statement.Methods)
		{
			ResolveFunction(method, FunctionType.Method);
		}

		EndScope();

		if (statement.Superclass is not null)
		{
			EndScope();
		}

		_currentClass = enclosing;
	}

	private void Resolve(ImportStatement statement)
	{
        foreach (var import in statement.Imports)
        {
            var name = ((IdentifierExpression)import).Name;
           
			Define(name);
            Declare(name);

			ResolveLocal(import, name);
        }
    }
}