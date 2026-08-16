using Vertical.Cli.Configuration.Assertion.Types;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Configuration.Assertion;

/// <summary>
/// Represents the contextual data by which configuration assertions can be built.
/// </summary>
public sealed class AssertionContext
{
    private readonly Dictionary<Type, ModelConfiguration> _cachedModelConfigurations = [];
    private readonly Dictionary<Command, ILookup<SymbolKind, ICliSymbol>> _cachedSymbols = [];
    
    internal AssertionContext(CommandLineApplication application)
    {
        Application = application;
        Commands = BuildCommands();
        CallSites = Commands.Where(command => command.CanCreateCallSite).ToArray();
        Directives = application
            .GetConfiguration()
            .GetMiddlewareSymbols()
            .Where(symbol => symbol.Kind == SymbolKind.Directive)
            .ToArray();
    }

    /// <summary>
    /// Gets the configuration's directives.
    /// </summary>
    public IReadOnlyList<ICliSymbol> Directives { get; set; }

    /// <summary>
    /// Gets the command call sites.
    /// </summary>
    public Command[] CallSites { get; set; }

    /// <summary>
    /// Gets the assertion list.
    /// </summary>
    public List<ConfigurationAssertion> Assertions { get; } = [];

    /// <summary>
    /// Gets the application's configuration.
    /// </summary>
    public IRootConfigurationView Configuration => Application.GetConfiguration();

    /// <summary>
    /// Gets the defined commands.
    /// </summary>
    public Command[] Commands { get; }

    /// <summary>
    /// Gets the command line application.
    /// </summary>
    public CommandLineApplication Application { get; }

    /// <summary>
    /// Gets a model configuration.
    /// </summary>
    /// <param name="modelType">The model type.</param>
    /// <returns><see cref="ModelConfiguration"/></returns>
    public ModelConfiguration GetModelConfiguration(Type modelType)
    {
        return _cachedModelConfigurations.GetOrAdd(modelType, () => Configuration.GetModelConfiguration(modelType));
    }

    /// <summary>
    /// Gets position arguments.
    /// </summary>
    /// <param name="command">The command to get position arguments of.</param>
    /// <returns>Enumeration of <see cref="CliSymbol"/></returns>
    public IEnumerable<CliSymbol> GetPositionArguments(Command command) =>
        GetSymbolLookup(Configuration, command)[SymbolKind.PositionArgument]
            .Cast<CliSymbol>();

    /// <summary>
    /// Gets named symbols (options, switches)
    /// </summary>
    /// <param name="command">The command to get named symbols for.</param>
    /// <returns>Enumeration of <see cref="CliSymbol"/></returns>
    public IEnumerable<ICliSymbol> GetNamedSymbols(Command command)
    {
        var symbols = GetSymbolLookup(Configuration, command);
        return symbols[SymbolKind.Option].Concat(symbols[SymbolKind.Switch]);
    }

    /// <summary>
    /// Gets all symbols.
    /// </summary>
    /// <param name="command">The command instance.</param>
    /// <returns>An enumeration of <see cref="CliSymbol"/> objects.</returns>
    public IEnumerable<ICliSymbol> GetSymbols(Command command)
    {
        var symbols = GetSymbolLookup(Configuration, command);
        return symbols.SelectMany(grouping => grouping);
    }

    private ILookup<SymbolKind, ICliSymbol> GetSymbolLookup(
        IRootConfigurationView configuration,
        Command command)
    {
        return _cachedSymbols.GetOrAdd(
            command,
            () => GetModelConfiguration(command.ModelType!)
                .BindingSources
                .OfType<CliSymbol>()
                .Cast<ICliSymbol>()
                .Concat(configuration.GetMiddlewareSymbols())
                .Append(Configuration.HelpOptions.CreateHelpSwitch())
                .ToLookup(symbol => symbol.Kind));
    }

    private Command[] BuildCommands()
    {
        return Enumerate(Application.RootCommand).ToArray();

        IEnumerable<Command> Enumerate(Command command) => command
            .SubCommands
            .SelectMany(Enumerate)
            .Append(command);
    }
}