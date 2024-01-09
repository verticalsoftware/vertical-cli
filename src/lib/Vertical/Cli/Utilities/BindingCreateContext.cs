using System.Collections.Immutable;
using Vertical.Cli.Configuration;
using Vertical.Cli.Conversion;
using Vertical.Cli.Parsing;
using Vertical.Cli.Validation;

namespace Vertical.Cli.Utilities;

public sealed class BindingCreateContext<TResult> : CommandPathContext<TResult>, IBindingCreateContext
{
    /// <inheritdoc />
    internal BindingCreateContext(
        IEnumerable<ICommandDefinition<TResult>> commands,
        IEnumerable<string> rawArguments,
        IEnumerable<string> subjectArguments) 
        : base(ImmutableArray.Create(commands.ToArray()))
    {
        RawArguments = rawArguments.ToArray();
        SubjectArguments = subjectArguments.ToArray();
        ArgumentSyntax = SubjectArguments.Select(SymbolSyntax.Parse).ToArray();
        SemanticArguments = new SemanticArgumentCollection(ArgumentSyntax);
        ConverterDictionary = Converters.ToMergedDictionary(converter => converter.ValueType);
        ValidatorDictionary = Validators.ToMergedDictionary(validator => validator.ValueType);
        SymbolIdentities = new HashSet<string>(Symbols.SelectMany(symbol => symbol.Identities));
        SymbolTypeLookup = Symbols.ToLookup(symbol => symbol.Type);
    }

    private ILookup<SymbolType, SymbolDefinition> SymbolTypeLookup { get; }

    /// <summary>
    /// Gets the raw arguments.
    /// </summary>
    public string[] RawArguments { get; }

    /// <summary>
    /// Gets the arguments appropriate for the command subject.
    /// </summary>
    public string[] SubjectArguments { get; }

    /// <summary>
    /// Gets the subject argument syntax.
    /// </summary>
    public SymbolSyntax[] ArgumentSyntax { get; }

    /// <summary>
    /// Gets the semantic arguments.
    /// </summary>
    public SemanticArgumentCollection SemanticArguments { get; }

    /// <summary>
    /// Gets the converter dictionary.
    /// </summary>
    public IReadOnlyDictionary<Type, ValueConverter> ConverterDictionary { get; }
    
    /// <summary>
    /// Gets the validators dictionary.
    /// </summary>
    public IReadOnlyDictionary<Type, Validator> ValidatorDictionary { get; }

    /// <inheritdoc />
    public IReadOnlyCollection<string> SymbolIdentities { get; }

    /// <summary>
    /// Gets the argument symbols.
    /// </summary>
    public IEnumerable<SymbolDefinition> ArgumentSymbols => SymbolTypeLookup[SymbolType.Argument];

    /// <summary>
    /// Gets the switch symbols.
    /// </summary>
    public IEnumerable<SymbolDefinition> SwitchSymbols => SymbolTypeLookup[SymbolType.Switch];

    /// <summary>
    /// Gets the option symbols.
    /// </summary>
    public IEnumerable<SymbolDefinition> OptionSymbols => SymbolTypeLookup[SymbolType.Option];

    /// <inheritdoc />
    public SymbolDefinition? HelpOptionSymbol => SymbolTypeLookup[SymbolType.HelpOption]
        .SingleOrDefault();
}