using Nitrogen.Core;
using Nitrogen.Core.Syntax.Expressions.Abstractions;

namespace Nitrogen.Core.Syntax.Expressions;

public class SuperExpression : IExpression
{
    public SuperExpression(Token keyword, List<IExpression> parameters)
    {
        Keyword = keyword;
        Parameters = parameters;
        Type = SuperType.Constructor;
    }

    public SuperExpression(Token keyword, Token member)
    {
        Keyword = keyword;
        Member = member;
        Type = SuperType.Accessor;
    }

    #region Constructor

    public List<IExpression> Parameters { get; } = default!;

    #endregion Constructor

    #region Accessor

    public Token Member { get; }

    #endregion Accessor

    public Token Keyword { get; }
    public SuperType Type { get; set; }
}

public enum SuperType
{
    Constructor,
    Accessor
}