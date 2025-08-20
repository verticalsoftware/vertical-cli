namespace Vertical.Cli.Parsing;

/// <summary>
/// Defines the options used by the parser.
/// </summary>
public interface IParserOptions
{
    /// <summary>
    /// Gets/sets the default symbol convention used when generating symbols.
    /// </summary>
    SymbolConvention DefaultSymbolConvention { get; set; }

    /// <summary>
    /// Gets/sets whether to parse directives. If <c>false</c>, the parser will treat matching
    /// strings as argument values.
    /// </summary>
    bool ParseDirectives { get; set; }

    /// <summary>
    /// Gets/sets whether to parse forward slash options. If <c>false</c>, the parser will treat matching
    /// strings as argument values.
    /// </summary>
    bool ParseWindowsStyleOptions { get; set; }

    /// <summary>
    /// Gets/sets whether to parse posix groups. If <c>false</c>, the parser will treat matching
    /// strings as argument values.
    /// </summary>
    bool ParsePosixGroups { get; set; }

    /// <summary>
    /// Gets/sets whether the parser ignores tokens that are matched to defined symbols.
    /// </summary>
    bool IgnorePendingTokens { get; set; }
}