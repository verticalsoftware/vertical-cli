namespace Vertical.Cli.Parsing;

/// <summary>
/// Represents an object that parses strings into meaningful command line objects.
/// </summary>
public interface IParser
{
    /// <summary>
    /// Generates a symbol from the given name.
    /// </summary>
    /// <param name="name">The name used to generate the symbol.</param>
    /// <returns><see cref="string"/></returns>
    string CreateSymbol(string name);

    /// <summary>
    /// Creates a collection of tokens.
    /// </summary>
    /// <param name="arguments">The arguments to parse.</param>
    /// <returns>Enumerator that yields <see cref="Token"/> objects.</returns>
    IEnumerable<Token> ParseArguments(IEnumerable<string> arguments);

    /// <summary>
    /// Creates a collection of semantic tokens.
    /// </summary>
    /// <param name="token">The original parsed token.</param>
    SemanticToken[] CreateSemanticTokens(Token token);
    
    /// <summary>
    /// Gets the configured options.
    /// </summary>
    IParserOptions Options { get; }

    /// <summary>
    /// Gets whether the given value matches the terminator token.
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns><see cref="bool"/>></returns>
    bool IsTerminatorToken(string value);
}