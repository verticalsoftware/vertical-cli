using System.Reflection;
using Vertical.Cli.Help;
using Vertical.Cli.IO;
using Vertical.Cli.Parsing;

namespace Vertical.Cli;

/// <summary>
/// Represents configuration options for the application.
/// </summary>
public sealed class CommandLineOptions : IParserOptions
{
    private SymbolConvention _defaultSymbolConvention = SymbolConvention.GnuOption;
    private string[] _helpOptionAliases = ["--help", "-?"];
    private object _helpOptionHelpTag = "Display help for the application or command";
    private string[] _versionOptionAliases = ["--version"];
    private object _versionOptionHelpTag = "Display the current version of the application";
    private Func<IConsole, IHelpProvider> _helpProviderFactory = DefaultHelpProvider.CreateDefault;
    
    /// <summary>
    /// Gets/sets the default symbol convention used when generating symbols.
    /// </summary>
    public SymbolConvention DefaultSymbolConvention
    {
        get => _defaultSymbolConvention;
        set => _defaultSymbolConvention = value switch
        {
            SymbolConvention.GnuOption => value,
            SymbolConvention.PosixOption => value,
            SymbolConvention.ForwardSlashOption => value,
            _ => throw new ArgumentException($"Invalid default symbol convention '{value}'")
        };
    }

    /// <summary>
    /// Gets/sets whether to parse directives. If <c>false</c>, the parser will treat matching
    /// strings as argument values.
    /// </summary>
    public bool ParseDirectives { get; set; } = true;

    /// <summary>
    /// Gets/sets whether to parse forward slash options. If <c>false</c>, the parser will treat matching
    /// strings as argument values.
    /// </summary>
    public bool ParseWindowsStyleOptions { get; set; } = false;

    /// <summary>
    /// Gets/sets whether to parse posix groups. If <c>false</c>, the parser will treat matching
    /// strings as argument values.
    /// </summary>
    public bool ParsePosixGroups { get; set; } = true;
    
    /// <summary>
    /// Gets/sets whether the parser ignores tokens that are matched to defined symbols.
    /// </summary>
    public bool IgnorePendingTokens { get; set; }

    /// <summary>
    /// Gets the help tag for the version option.
    /// </summary>
    public string[] HelpOptionAliases
    {
        get => _helpOptionAliases;
        set => _helpOptionAliases = (value ?? throw new ArgumentNullException(nameof(value))) is { Length: > 0 }
            ? value
            : throw new ArgumentException("array must contain one identifier", nameof(value));
    }

    /// <summary>
    /// Gets the help tag for the version option.
    /// </summary>
    public object HelpOptionHelpTag
    {
        get => _helpOptionHelpTag;
        set => _helpOptionHelpTag = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Gets the help tag for the version option.
    /// </summary>
    public string[] VersionOptionAliases
    {
        get => _versionOptionAliases;
        set => _versionOptionAliases = (value ?? throw new ArgumentNullException(nameof(value))) is { Length: > 0 }
            ? value
            : throw new ArgumentException("array must contain one identifier", nameof(value));
    }

    /// <summary>
    /// Gets the help tag for the version option.
    /// </summary>
    public object VersionOptionHelpTag
    {
        get => _versionOptionHelpTag;
        set => _versionOptionHelpTag = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Gets the application version to display.
    /// </summary>
    public string? ApplicationVersion { get; set; } = DefaultApplicationVersion;

    /// <summary>
    /// Gets a function that creates the help provider.
    /// </summary>
    public Func<IConsole, IHelpProvider> HelpProviderFactory
    {
        get => _helpProviderFactory;
        set => _helpProviderFactory = value ?? throw new ArgumentNullException(nameof(value));
    }

    private static string? DefaultApplicationVersion => Assembly
        .GetEntryAssembly()?
        .GetName()
        .Version?
        .ToString();
}