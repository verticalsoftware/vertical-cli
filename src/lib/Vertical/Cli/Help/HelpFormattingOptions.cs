using Vertical.Cli.Configuration;
using Vertical.Cli.Routing;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Help;

/// <summary>
/// Defines options used when rendering help.
/// </summary>
public sealed class HelpFormattingOptions
{
    private const string DefaultFg = "\x1b[0m";
    private const string BrightFg = "\x1b[38;5;15m";
    private const string SubduedFg = "\x1b[38;5;250m";
    private const string DimFg = "\x1b[38;5;246m";
    private const string OperandFg = "\x1b[38;5;136m";
    private const string UsageFg = "\x1b[38;5;79m";

    /// <summary>
    /// Defines the default instance.
    /// </summary>
    public static readonly HelpFormattingOptions Default = new();
    
    /// <summary>
    /// Gets the number of spaces to write for each indent level.
    /// </summary>
    public int IndentSpaces { get; init; } = 3;

    /// <summary>
    /// Gets a function that formats help elements after positioning but before rendering to
    /// the underlying <see cref="TextWriter"/>
    /// </summary>
    public Func<HelpElement, string, string> OutputFormatter { get; init; } = (element, str) =>
    {
        var fg = element switch
        {
            HelpElement.SectionTitle => BrightFg,
            HelpElement.OperandSyntax => OperandFg,
            HelpElement.UsageIdentifier => UsageFg,
            HelpElement.IdentifierList => SubduedFg,
            _ => DimFg
        };

        return $"{fg}{str}{DefaultFg}";
    };
    
    /// <summary>
    /// Gets a function that formats section titles.
    /// </summary>
    public Func<HelpSection, string> SectionTitleFormatter { get; init; } = section => section switch
    {
        HelpSection.Description => "Description:",
        HelpSection.Usage => "Usage:",
        _ => string.Empty
    };

    /// <summary>
    /// Gets a function that formats route descriptions.
    /// </summary>
    public Func<RouteDefinition, string> RouteDescriptionFormatter { get; init; } =
        route => route.HelpTag as string ?? string.Empty;
    
    /// <summary>
    /// Gets a function that formats argument usage syntax.
    /// </summary>
    public Func<RouteDefinition, CliParameter[], string> ArgumentsUsageFormatter { get; init; } = 
        (_, arguments) => arguments.Length > 3 
            ? "[Arguments]" 
            : string.Join(' ', arguments.OrderBy(arg => arg.Index).Select(CreateArgumentUsageSyntax));

    /// <summary>
    /// Gets a function that formats options usage syntax.
    /// </summary>
    public Func<RouteDefinition, CliParameter[], string> OptionsUsageFormatter { get; init; } =
        (_, _) => "[Options]";

    /// <summary>
    /// Gets a function that formats sub route usage syntax.
    /// </summary>
    public Func<RouteDefinition, string> SubRouteUsageFormatter { get; init; } =
        route => route.IsCallable ? "[Command]" : "<Command>";

    /// <summary>
    /// Gets a function that categorizes routes into groups. Each key serves as the group's section title.
    /// </summary>
    public Func<RouteDefinition[], IEnumerable<IGrouping<string, RouteDefinition>>> RouteGroupsProvider { get; init; } =
        routes => routes.GroupBy(_ => "Commands:");

    /// <summary>
    /// Gets a function that categorizes options into groups. Each key serves as the group's section title.
    /// </summary>
    public Func<CliParameter[], IEnumerable<IGrouping<string, CliParameter>>> OptionsGroupsProvider { get; init; } =
        options => options.Order(CliParameterIdSortComparer.Instance).GroupBy(_ => "Options:");

    /// <summary>
    /// Gets a function that categorizes arguments into groups. Each key serves as the group's section title.
    /// </summary>
    public Func<CliParameter[], IEnumerable<IGrouping<string, CliParameter>>> ArgumentGroupsProvider { get; init; } =
        arguments => arguments.GroupBy(_ => "Arguments:");

    /// <summary>
    /// Gets a function that formats identifier lists.
    /// </summary>
    public Func<CliParameter, string> IdentifierListFormatter { get; init; } = parameter =>
        parameter.SymbolKind == SymbolKind.Argument
            ? parameter.BindingName
            : string.Join(", ", parameter.Identifiers.Select(id => id.Text));
    
    /// <summary>
    /// Gets a function that formats parameter operands.
    /// </summary>
    public Func<CliParameter, string> OperandFormatter { get; init; } = parameter => parameter switch
    {
        { SymbolKind: SymbolKind.Argument } => parameter.BindingName,
        { SymbolKind: SymbolKind.Option, Arity.MaxCount: null } => $"<{parameter.BindingName.SnakeCase()}>...",
        { SymbolKind: SymbolKind.Option } => $"<{parameter.BindingName.SnakeCase()}>",
        _ => string.Empty
    };

    /// <summary>
    /// Gets a function that formats parameter descriptions.
    /// </summary>
    public Func<CliParameter, string> ParameterDescriptionFormatter { get; init; } = parameter =>
        parameter.HelpTag as string ?? string.Empty;
    
    private static string CreateArgumentUsageSyntax(CliParameter arg)
    {
        var wrappingTokens = arg.Arity.MinCount > 0 ? "<>" : "[]";
        var many = arg.Arity.MaxCount == null ? "..." : string.Empty;

        return $"{wrappingTokens[0]}{arg.BindingName}{many}{wrappingTokens[1]}";
    }
}