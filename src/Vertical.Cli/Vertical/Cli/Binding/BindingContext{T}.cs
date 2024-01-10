using Vertical.Cli.Configuration;
using Vertical.Cli.Conversion;
using Vertical.Cli.Invocation;
using Vertical.Cli.Parsing;
using Vertical.Cli.Utilities;
using Vertical.Cli.Validation;

namespace Vertical.Cli.Binding;

internal sealed class BindingContext<TResult> : IBindingContext<TResult>
{
    internal BindingContext(
        BindingCreateContext<TResult> createContext,
        IEnumerable<ArgumentBinding> bindings,
        ICallSite<TResult> callSite,
        Exception? bindingException)
    {
        CallSite = callSite;
        OriginalSemanticArguments = new SemanticArgumentCollection(createContext.SemanticArguments);
        CreateContext = createContext;
        BindingDictionary = bindings.ToDictionary(
            binding => binding.BindingId,
            BindingIdComparer.Default);
        BindingException = bindingException;
    }

    private BindingCreateContext<TResult> CreateContext { get; }

    /// <inheritdoc />
    public ICallSite<TResult> CallSite { get; }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, ArgumentBinding> BindingDictionary { get; }

    /// <inheritdoc />
    public ArgumentBinding<T> GetBinding<T>(string bindingId) => (ArgumentBinding<T>)BindingDictionary[bindingId];

    /// <inheritdoc />
    public T GetValue<T>(string bindingId) => GetBinding<T>(bindingId).Values.SingleOrDefault()!;

    /// <inheritdoc />
    public IEnumerable<T> GetValues<T>(string bindingId) => GetBinding<T>(bindingId).Values;

    /// <inheritdoc />
    public string[] RawArguments => CreateContext.RawArguments;

    /// <inheritdoc />
    public string[] SubjectArguments => CreateContext.SubjectArguments;

    /// <inheritdoc />
    public SymbolSyntax[] ArgumentSyntax => CreateContext.ArgumentSyntax;

    /// <inheritdoc />
    public SemanticArgumentCollection SemanticArguments => CreateContext.SemanticArguments;

    /// <inheritdoc />
    public SemanticArgumentCollection OriginalSemanticArguments { get; }

    /// <inheritdoc />
    public ICommandDefinition<TResult> Subject => CreateContext.Subject;

    /// <inheritdoc />
    public Exception? BindingException { get; }
    
    /// <inheritdoc />
    public IReadOnlyDictionary<Type, ValueConverter> ConverterDictionary => CreateContext.ConverterDictionary;

    /// <inheritdoc />
    public IReadOnlyDictionary<Type, Validator> ValidatorDictionary => CreateContext.ValidatorDictionary;

    /// <inheritdoc />
    public IReadOnlyCollection<string> SymbolIdentities => CreateContext.SymbolIdentities;

    /// <inheritdoc />
    public IEnumerable<SymbolDefinition> ArgumentSymbols => CreateContext.ArgumentSymbols;

    /// <inheritdoc />
    public IEnumerable<SymbolDefinition> SwitchSymbols => CreateContext.SwitchSymbols;

    /// <inheritdoc />
    public IEnumerable<SymbolDefinition> OptionSymbols => CreateContext.OptionSymbols;

    /// <inheritdoc />
    public SymbolDefinition? HelpOptionSymbol => CreateContext.HelpOptionSymbol;
}