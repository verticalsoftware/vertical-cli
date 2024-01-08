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
        IEnumerable<string> args)
        where TModel : class
    {
        return BindingContextFactory.Create(rootCommand, args);
    }
}

internal sealed class BindingContext<TResult> : IBindingContext<TResult>
{
    internal BindingContext(
        BindingCommandPath<TResult> commandPath,
        IEnumerable<ArgumentBinding> bindings,
        Exception? bindingException)
    {
        OriginalSemanticArguments = new SemanticArgumentCollection(commandPath.SemanticArguments);
        CommandPath = commandPath;
        BindingDictionary = bindings.ToDictionary(
            binding => binding.BindingId,
            BindingIdComparer.Default);
        BindingException = bindingException;
    }

    private BindingCommandPath<TResult> CommandPath { get; }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, ArgumentBinding> BindingDictionary { get; }

    /// <inheritdoc />
    public ArgumentBinding<T> GetBinding<T>(string bindingId) => (ArgumentBinding<T>)BindingDictionary[bindingId];

    /// <inheritdoc />
    public T GetValue<T>(string bindingId) => GetBinding<T>(bindingId).Values.SingleOrDefault()!;

    /// <inheritdoc />
    public IEnumerable<T> GetValues<T>(string bindingId) => GetBinding<T>(bindingId).Values;

    /// <inheritdoc />
    public string[] RawArguments => CommandPath.RawArguments;

    /// <inheritdoc />
    public string[] SubjectArguments => CommandPath.SubjectArguments;

    /// <inheritdoc />
    public SymbolSyntax[] ArgumentSyntax => CommandPath.ArgumentSyntax;

    /// <inheritdoc />
    public SemanticArgumentCollection SemanticArguments => CommandPath.SemanticArguments;

    /// <inheritdoc />
    public SemanticArgumentCollection OriginalSemanticArguments { get; }

    /// <inheritdoc />
    public ICommandDefinition<TResult> Subject => CommandPath.Subject;

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
    public Func<TResult> CreateCallSite<TModel>(TModel model) where TModel : class
    {
        var commandDefinition = (ICommandDefinition<TModel, TResult>)Subject;
        var handler = commandDefinition.Handler;

        return () => handler!(model);
    }

    /// <inheritdoc />
    public IReadOnlyDictionary<Type, ValueConverter> ConverterDictionary => CommandPath.ConverterDictionary;

    /// <inheritdoc />
    public IReadOnlyDictionary<Type, Validator> ValidatorDictionary => CommandPath.ValidatorDictionary;

    /// <inheritdoc />
    public IReadOnlyCollection<string> SymbolIdentities => CommandPath.SymbolIdentities;

    /// <inheritdoc />
    public IEnumerable<SymbolDefinition> ArgumentSymbols => CommandPath.ArgumentSymbols;

    /// <inheritdoc />
    public IEnumerable<SymbolDefinition> SwitchSymbols => CommandPath.SwitchSymbols;

    /// <inheritdoc />
    public IEnumerable<SymbolDefinition> OptionSymbols => CommandPath.OptionSymbols;
}