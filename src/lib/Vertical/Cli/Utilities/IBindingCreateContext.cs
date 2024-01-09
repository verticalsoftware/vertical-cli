using Vertical.Cli.Configuration;
using Vertical.Cli.Conversion;
using Vertical.Cli.Parsing;
using Vertical.Cli.Validation;

namespace Vertical.Cli.Utilities;

/// <summary>
/// Provides binding data for a command path.
/// </summary>
public interface IBindingCreateContext
{
    /// <summary>
    /// Gets the raw arguments.
    /// </summary>
    string[] RawArguments { get; }

    /// <summary>
    /// Gets the arguments appropriate for the command subject.
    /// </summary>
    string[] SubjectArguments { get; }

    /// <summary>
    /// Gets the subject argument syntax.
    /// </summary>
    SymbolSyntax[] ArgumentSyntax { get; }

    /// <summary>
    /// Gets the semantic arguments.
    /// </summary>
    SemanticArgumentCollection SemanticArguments { get; }

    /// <summary>
    /// Gets the converter dictionary.
    /// </summary>
    IReadOnlyDictionary<Type, ValueConverter> ConverterDictionary { get; }

    /// <summary>
    /// Gets the validators dictionary.
    /// </summary>
    IReadOnlyDictionary<Type, Validator> ValidatorDictionary { get; }
    
    /// <summary>
    /// Gets a collection of all symbol identities in the path.
    /// </summary>
    IReadOnlyCollection<string> SymbolIdentities { get; }

    /// <summary>
    /// Gets the argument symbols.
    /// </summary>
    IEnumerable<SymbolDefinition> ArgumentSymbols { get; }

    /// <summary>
    /// Gets the switch symbols.
    /// </summary>
    IEnumerable<SymbolDefinition> SwitchSymbols { get; }

    /// <summary>
    /// Gets the option symbols.
    /// </summary>
    IEnumerable<SymbolDefinition> OptionSymbols { get; }
    
    /// <summary>
    /// Gets the help option symbol, or <c>null</c>.
    /// </summary>
    SymbolDefinition? HelpOptionSymbol { get; }
}