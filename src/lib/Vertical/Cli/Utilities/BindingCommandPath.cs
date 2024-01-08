using System.Collections.Immutable;
using Vertical.Cli.Configuration;
using Vertical.Cli.Conversion;
using Vertical.Cli.Parsing;
using Vertical.Cli.Validation;

namespace Vertical.Cli.Utilities;

public sealed class BindingCommandPath<TResult> : CommandPath<TResult>, IBindingPath
{
    /// <inheritdoc />
    internal BindingCommandPath(
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
    }

    /// <summary>
    /// Gets the raw arguments.
    /// </summary>
    public string[] RawArguments { get; set; }

    /// <summary>
    /// Gets the arguments appropriate for the command subject.
    /// </summary>
    public string[] SubjectArguments { get; set; }

    /// <summary>
    /// Gets the subject argument syntax.
    /// </summary>
    public SymbolSyntax[] ArgumentSyntax { get; set; }

    /// <summary>
    /// Gets the semantic arguments.
    /// </summary>
    public SemanticArgumentCollection SemanticArguments { get; set; }

    /// <summary>
    /// Gets the converter dictionary.
    /// </summary>
    public IReadOnlyDictionary<Type, ValueConverter> ConverterDictionary { get; set; }
    
    /// <summary>
    /// Gets the validators dictionary.
    /// </summary>
    public IReadOnlyDictionary<Type, Validator> ValidatorDictionary { get; set; }

    /// <inheritdoc />
    public IReadOnlyCollection<string> SymbolIdentities { get; }

    /// <summary>
    /// Gets the argument symbols.
    /// </summary>
    public IEnumerable<SymbolDefinition> ArgumentSymbols => Symbols.Where(symbol => symbol.Type == SymbolType.Argument);

    /// <summary>
    /// Gets the switch symbols.
    /// </summary>
    public IEnumerable<SymbolDefinition> SwitchSymbols => Symbols.Where(symbol => symbol.Type == SymbolType.Switch);

    /// <summary>
    /// Gets the option symbols.
    /// </summary>
    public IEnumerable<SymbolDefinition> OptionSymbols => Symbols.Where(symbol => symbol.Type == SymbolType.Option);
}