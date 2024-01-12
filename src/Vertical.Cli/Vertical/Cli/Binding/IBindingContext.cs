using Vertical.Cli.Configuration;
using Vertical.Cli.Conversion;
using Vertical.Cli.Parsing;
using Vertical.Cli.Validation;

namespace Vertical.Cli.Binding;

public interface IBindingContext
{
    /// <summary>
    /// Gets the command subject.
    /// </summary>
    ICommandDefinition Subject { get; }
    
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
    /// Gets a collection of the original semantic arguments not consumed by parsing.
    /// </summary>
    SemanticArgumentCollection OriginalSemanticArguments { get; }

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
    
    /// <summary>
    /// Gets the bindings available in the context.
    /// </summary>
    IReadOnlyDictionary<string, ArgumentBinding> BindingDictionary { get; }
    
    /// <summary>
    /// Gets a lookup of binding values where the key is the symbol identity.
    /// </summary>
    BindingLookup BindingValueLookup { get; }
    
    /// <summary>
    /// Gets a binding that matches the given id.
    /// </summary>
    /// <param name="bindingId">Binding id to match.</param>
    /// <typeparam name="T">Expected value type.</typeparam>
    /// <returns><see cref="ArgumentBinding{T}"/></returns>
    /// <exception cref="InvalidOperationException">The binding is not found.</exception>
    ArgumentBinding<T> GetBinding<T>(string bindingId);

    /// <summary>
    /// Gets the single binding value.
    /// </summary>
    /// <param name="bindingId">Binding id to match.</param>
    /// <typeparam name="T">Expected value type.</typeparam>
    /// <returns>The single value.</returns>
    T GetValue<T>(string bindingId);

    /// <summary>
    /// Gets multiple binding values.
    /// </summary>
    /// <param name="bindingId">Binding id to match.</param>
    /// <typeparam name="T">Expected value type.</typeparam>
    /// <returns>The binding values.</returns>
    IEnumerable<T> GetValues<T>(string bindingId);
}