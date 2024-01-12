using Vertical.Cli.Configuration;
using Vertical.Cli.Conversion;
using Vertical.Cli.Parsing;
using Vertical.Cli.Validation;

namespace Vertical.Cli.Binding;

internal sealed class BindingContextBuilder : IBindingContext
{
    private readonly Dictionary<string, ArgumentBinding> _bindings = new(BindingIdComparer.Default);
    
    internal BindingContextBuilder(
        CliOptions options,
        ICommandDefinition subject,
        IEnumerable<string> rawArguments,
        IEnumerable<string> subjectArguments)
    {
        Options = options;
        Subject = subject;
        RawArguments = rawArguments.ToArray();
        SubjectArguments = subjectArguments.ToArray();
        ArgumentSyntax = SubjectArguments.Select(SymbolSyntax.Parse).ToArray();
        SemanticArguments = new SemanticArgumentCollection(ArgumentSyntax);
        OriginalSemanticArguments = new SemanticArgumentCollection(ArgumentSyntax);
        ConverterDictionary = options.Converters.ToDictionary(converter => converter.ValueType);
        ValidatorDictionary = options.Validators.ToDictionary(validator => validator.ValueType);

        var symbols = subject.GetInheritedSymbols().Concat(subject.GetScopedSymbols());
        SymbolLookup = symbols.ToLookup(symbol => symbol.Type);
        SymbolIdentities = new HashSet<string>(symbols.SelectMany(symbol => symbol.Identities));
    }

    private ILookup<SymbolType, SymbolDefinition> SymbolLookup { get; }

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
    public IEnumerable<SymbolDefinition> ArgumentSymbols => SymbolLookup[SymbolType.Argument];

    /// <inheritdoc />
    public IEnumerable<SymbolDefinition> SwitchSymbols => SymbolLookup[SymbolType.Switch];

    /// <inheritdoc />
    public IEnumerable<SymbolDefinition> OptionSymbols => SymbolLookup[SymbolType.Option];

    /// <inheritdoc />
    public SymbolDefinition? HelpOptionSymbol => SymbolLookup[SymbolType.HelpOption].FirstOrDefault();

    /// <inheritdoc />
    public IReadOnlyDictionary<string, ArgumentBinding> BindingDictionary => _bindings;

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

    /// <inheritdoc />
    public SemanticArgumentCollection OriginalSemanticArguments { get; }

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