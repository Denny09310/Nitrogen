using Nitrogen.Exceptions;
using Nitrogen.Syntax;
using Nitrogen.Syntax.Abstractions;
using Nitrogen.Syntax.Expressions;
using Nitrogen.Syntax.Statements;

using System.Diagnostics;

namespace Nitrogen.Interpreting.Binding;

internal class LoopResolver(Resolver resolver)
{
    private Loop? _current;

    public void Resolve(BreakExpression expression)
    {
        if (_current != null)
        {
            _current.Infinite = false;
        }
    }

    public void ResolveLoop(IStatement statement, LoopType type)
    {
        var enclosing = _current;
        _current = new Loop(type);

        Token token;
        if (statement is WhileStatement @while)
        {
            token = @while.Keyword;

            resolver.Resolve(@while.Condition);
            resolver.Resolve(@while.Body);

            if (@while.Condition is not LiteralExpression expression || expression.Literal is not bool bool1 || !bool1)
            {
                _current.Infinite = false;
            }
        }
        else if (statement is ForStatement @for)
        {
            token = @for.Keyword;

            if (@for.Initialization is not null) resolver.Resolve(@for.Initialization);
            resolver.Resolve(@for.Condition);
            resolver.Resolve(@for.Body);

            if (@for.Increment is not null)
            {
                _current.Infinite = false;

                resolver.Resolve(@for.Increment);
            }
        }
        else
        {
            throw new UnreachableException("Non loop statement detected.");
        }

        if (_current.Infinite)
        {
            resolver.Error(ExceptionLevel.Warning, token, "Possible infinite loop statement detected.");
        }

        _current = enclosing;
    }

    private sealed class Loop(LoopType type)
    {
        public bool Infinite { get; set; } = true;
        public LoopType Type { get; init; } = type;
    }
}