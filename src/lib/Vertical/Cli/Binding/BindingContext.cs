using Vertical.Cli.Configuration;
using Vertical.Cli.Conversion;
using Vertical.Cli.Parsing;
using Vertical.Cli.Utilities;
using Vertical.Cli.Validation;

namespace Vertical.Cli.Binding;

public static class BindingContext
{
    public static IBindingContext<TResult> Create<TModel, TResult>(
        IRootCommand<TModel, TResult> rootCommand,
        IEnumerable<string> args,
        TResult defaultValue)
        where TModel : class
    {
        return BindingContextFactory.Create(rootCommand, args, defaultValue);
    }
}

internal sealed class BindingContext<TResult> : IBindingContext<TResult>
{
    internal BindingContext(
        BindingCreateContext<TResult> createContext,
        IEnumerable<ArgumentBinding> bindings,
        Func<TResult>? helpCallSite,
        Exception? bindingException)
    {
        OriginalSemanticArguments = new SemanticArgumentCollection(createContext.SemanticArguments);
        CreateContext = createContext;
        HelpCallSite = helpCallSite;
        BindingDictionary = bindings.ToDictionary(
            binding => binding.BindingId,
            BindingIdComparer.Default);
        BindingException = bindingException;
    }

    private BindingCreateContext<TResult> CreateContext { get; }
    
    public Func<TResult>? HelpCallSite { get; }

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
    public void ThrowBindingExceptions()
    {
        if (BindingException == null)
            return;

        throw BindingException;
    }

    /// <inheritdoc />
    public bool IsHelpCallSite => HelpCallSite != null;

    /// <inheritdoc />
    public Type CallSiteModelType => HelpCallSite != null ? typeof(None) : Subject.ModelType;

    /// <inheritdoc />
    public Func<TResult> CreateCallSite<TModel>(TModel model) where TModel : class
    {
        return HelpCallSite ?? WrapCallSite(model);
    }

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
    
    private Func<TResult> WrapCallSite<TModel>(TModel model) where TModel : class
    {
        var commandDefinition = (ICommandDefinition<TModel, TResult>)Subject;
        var handler = commandDefinition.Handler;

        return () => handler!(model);
    }
}