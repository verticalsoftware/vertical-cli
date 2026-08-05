using Vertical.Cli.Configuration.Assertion.Types;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Configuration.Assertion;

internal sealed class AssertionContext
{
    private readonly Dictionary<Type, ModelConfiguration> _cachedModelConfigurations = [];
    private readonly Dictionary<Command, ILookup<SymbolKind, CliSymbol>> _cachedSymbols = [];
    
    public AssertionContext(CommandLineApplication application)
    {
        Application = application;
        Commands = BuildCommands();
        CallSites = Commands.Where(command => command.CanCreateCallSite).ToArray();
        Directives = application.GetConfiguration().GetDirectives();
    }

    public IReadOnlyList<IDirectiveSymbol> Directives { get; set; }

    public Command[] CallSites { get; set; }

    public List<ConfigurationAssertion> Assertions { get; } = [];

    public IRootConfigurationView Configuration => Application.GetConfiguration();

    public Command[] Commands { get; set; }

    public CommandLineApplication Application { get; }
    
    private Command[] BuildCommands()
    {
        return Enumerate(Application.RootCommand).ToArray();

        IEnumerable<Command> Enumerate(Command command) => command
            .SubCommands
            .SelectMany(Enumerate)
            .Append(command);
    }

    public ModelConfiguration GetModelConfiguration(Type modelType)
    {
        return _cachedModelConfigurations.GetOrAdd(modelType, () => Configuration.GetModelConfiguration(modelType));
    }

    public IEnumerable<CliSymbol> GetPositionArguments(Command command) => 
        GetSymbolLookup(command)[SymbolKind.PositionArgument];

    public IEnumerable<CliSymbol> GetNamedSymbols(Command command)
    {
        var symbols = GetSymbolLookup(command);
        return symbols[SymbolKind.Option].Concat(symbols[SymbolKind.Switch]);
    }

    public IEnumerable<CliSymbol> GetSymbols(Command command)
    {
        var symbols = GetSymbolLookup(command);
        return symbols.SelectMany(grouping => grouping);
    }

    private ILookup<SymbolKind, CliSymbol> GetSymbolLookup(Command command)
    {
        return _cachedSymbols.GetOrAdd(
            command,
            () => GetModelConfiguration(command.ModelType!)
                .BindingSources
                .OfType<CliSymbol>()
                .ToLookup(symbol => symbol.Kind));
    }
}