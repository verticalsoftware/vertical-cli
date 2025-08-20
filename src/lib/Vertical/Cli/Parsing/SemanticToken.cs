using Vertical.Cli.Internal;

namespace Vertical.Cli.Parsing;

/// <summary>
/// Represents a token that may have additional semantic meaning then it's constituent token.
/// </summary>
/// <param name="Token">The semantic token representation.</param>
/// <param name="ConstituentToken">The underlying constituent token</param>
public record SemanticToken(Token Token, Token ConstituentToken)
{
    /// <inheritdoc />
    public override string ToString() => $"{Token}";

    /// <summary>
    /// Gets whether the attached parameter value is a boolean value.
    /// </summary>
    public bool HasBooleanParameter
    {
        get
        {
            var syntax = Token.Syntax;
            
            return  syntax.HasParameter &&
                    (syntax.ParameterSpan.SequenceEqual("true", CaseInsensitiveCharComparer.Default)
                     ||
                     syntax.ParameterSpan.SequenceEqual("false", CaseInsensitiveCharComparer.Default));
        }
    }
}