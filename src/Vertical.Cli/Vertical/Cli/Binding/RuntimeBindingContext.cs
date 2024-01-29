using Vertical.Cli.Configuration;
using Vertical.Cli.Conversion;
using Vertical.Cli.Parsing;
using Vertical.Cli.Validation;

namespace Vertical.Cli.Binding;

internal sealed class RuntimeBindingContext : IBindingContext
{
    private readonly Dictionary<string, ArgumentBinding> _bindings = new(BindingIdComparer.Default);
    
    internal RuntimeBindingContext(
        CliOptions options,
        ICommandDefinition subject,
        IEnumerable<string> rawArguments,
        IEnumerable<string> subjectArguments)
    {
        var symbols = subject.GetAllSymbols();

        Options = options;
        Subject = subject;
        RawArguments = rawArguments.ToArray();
        SubjectArguments = subjectArguments.ToArray();
        ArgumentSyntax = SubjectArguments.Select(SymbolSyntax.Parse).ToArray();
        SemanticArguments = new SemanticArgumentCollection(subject.GetAllSymbols(), ArgumentSyntax);
        ConverterDictionary = options.Converters.ToDictionary(converter => converter.ValueType);
        ValidatorDictionary = options.Validators.ToDictionary(validator => validator.ValueType);
        SymbolLookup = symbols.ToLookup(symbol => symbol.Kind);
        SymbolIdentities = new HashSet<string>(symbols.SelectMany(symbol => symbol.Identities));
        HelpOptionSymbol = (HelpSymbolDefinition?)symbols.FirstOrDefault(symbol => 
            symbol.SpecialType == SymbolSpecialType.HelpOption);
        ResponseFileOptionSymbol = (ResponseFileSymbolDefinition?)symbols.FirstOrDefault(symbol => 
            symbol.SpecialType == SymbolSpecialType.ResponseFileOption);
    }

    private ILookup<SymbolKind, SymbolDefinition> SymbolLookup { get; }

    /// <summary>
    /// Gets the CLI options.
    /// </summary>
    public CliOptions Options { get; }

    /// <inheritdoc />
    public ICommandDefinition Subject { get; }

    /// <inheritdoc />
    public string[] RawArguments { get; }

    /// <inheritdoc />
    public string[] SubjectArguments { get; }

    /// <inheritdoc />
    public SymbolSyntax[] ArgumentSyntax { get; }

    /// <inheritdoc />
    public SemanticArgumentCollection SemanticArguments { get; }

    /// <inheritdoc />
    public IReadOnlyDictionary<Type, ValueConverter> ConverterDictionary { get; }

    /// <inheritdoc />
    public IReadOnlyDictionary<Type, Validator> ValidatorDictionary { get; }

    /// <inheritdoc />
    public IReadOnlyCollection<string> SymbolIdentities { get; }

    /// <inheritdoc />
    public IEnumerable<SymbolDefinition> ArgumentSymbols => SymbolLookup[SymbolKind.Argument];

    /// <inheritdoc />
    public IEnumerable<SymbolDefinition> SwitchSymbols => SymbolLookup[SymbolKind.Switch];

    /// <inheritdoc />
    public IEnumerable<SymbolDefinition> OptionSymbols => SymbolLookup[SymbolKind.Option];

    /// <inheritdoc />
    public HelpSymbolDefinition? HelpOptionSymbol { get; }

    /// <inheritdoc />
    public ResponseFileSymbolDefinition? ResponseFileOptionSymbol { get; }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, ArgumentBinding> BindingDictionary => _bindings;

    /// <inheritdoc />
    public BindingLookup BindingValueLookup => new(BindingDictionary
        .SelectMany(kv => kv.Value.Select(obj => (kv.Key, Value: obj)))
        .ToLookup(kv => kv.Key, kv => kv.Value));

    /// <inheritdoc />
    public ArgumentBinding<T> GetBinding<T>(string bindingId)
    {
        return (ArgumentBinding<T>)_bindings[bindingId];
    }

    /// <inheritdoc />
    public T GetValue<T>(string bindingId)
    {
        return GetBinding<T>(bindingId).Values.SingleOrDefault()!;
    }

    /// <inheritdoc />
    public IEnumerable<T> GetValues<T>(string bindingId)
    {
        return GetBinding<T>(bindingId).Values;
    }

    internal void AddBinding(ArgumentBinding binding)
    {
        _bindings.Add(binding.BindingId, binding);
    }

    internal void AddBindings(IEnumerable<ArgumentBinding> bindings)
    {
        foreach (var binding in bindings)
        {
            AddBinding(binding);
        }
    }
}