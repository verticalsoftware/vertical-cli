namespace Vertical.Cli.Parsing;

/// <summary>
/// Extension methods for <see cref="Token"/>
/// </summary>
public static class TokenExtensions
{
    /// <summary>
    /// Gets a syntax structure specific to directives.
    /// </summary>
    /// <param name="token">The token instance.</param>
    /// <returns><see cref="DirectiveSyntax"/></returns>
    /// <exception cref="ArgumentException">The token is not a directive</exception>
    public static DirectiveSyntax GetDirectiveSyntax(this Token token) => DirectiveSyntax.Parse(token.Text);
}