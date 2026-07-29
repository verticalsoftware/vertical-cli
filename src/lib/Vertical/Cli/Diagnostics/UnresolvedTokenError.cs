using Vertical.Cli.Parsing;

namespace Vertical.Cli.Diagnostics;

/// <summary>
/// Indicates a token was not matched to a position argument, option, switch, or other
/// symbol.
/// </summary>
public sealed class UnresolvedTokenError : CommandLineError
{
    internal UnresolvedTokenError(ArgumentToken token) : base(FormatMessage(token))
    {
        Token = token;
    }

    /// <summary>
    /// Gets the token that could not be resolved.
    /// </summary>
    public ArgumentToken Token { get; }

    private static string FormatMessage(ArgumentToken token)
    {
        return token.Kind switch
        {
            TokenKind.CommandOrArgument or TokenKind.Argument => $"No sub command found matching '{token.Text}'.",
            TokenKind.Option => $"Invalid option or switch '{token.Symbol}'.",
            TokenKind.Annotation => "Annotated arguments are not supported by this application.",
            TokenKind.Directive => $"No directive found matching '{token.Symbol}'.",
            _ => $"Invalid argument '{token.Text}'."
        };
    }
}