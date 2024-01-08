using System.Diagnostics.CodeAnalysis;
using Vertical.Cli.Conversion;
using Vertical.Cli.Utilities;
using Vertical.Cli.Validation;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents a command definition.
/// </summary>
/// <typeparam name="TModel">The type of model used by the handler implementation.</typeparam>
/// <typeparam name="TResult">The type of result produced by the command handler.</typeparam>
internal class CommandDefinition<TModel, TResult> : ICommandDefinition<TModel, TResult>
    where TModel : class
{
    internal CommandDefinition(ICommandDefinition<TModel, TResult> definition) => Definition = definition;
    
    internal ICommandDefinition<TModel, TResult> Definition { get; }

    /// <inheritdoc />
    public string Id => Definition.Id;

    /// <inheritdoc />
    public Type ModelType => Definition.ModelType;

    /// <inheritdoc />
    public Type ResultType => Definition.ResultType;

    /// <inheritdoc />
    public IReadOnlyCollection<SymbolDefinition> Symbols => Definition.Symbols;

    /// <inheritdoc />
    public IReadOnlyCollection<ValueConverter> Converters => Definition.Converters;

    /// <inheritdoc />
    public IReadOnlyCollection<Validator> Validators => Definition.Validators;

    /// <inheritdoc />
    public bool HasHandler => Definition.HasHandler;

    /// <inheritdoc />
    public ICommandDefinition? Parent => Definition.Parent;

    /// <inheritdoc />
    public bool TryCreateChild(string id, [NotNullWhen(true)] out ICommandDefinition<TResult>? child) => 
        Definition.TryCreateChild(id, out child);

    /// <inheritdoc />
    public IEnumerable<ICommandDefinition<TResult>> SubCommands => Definition.SubCommands;

    /// <inheritdoc />
    public IEnumerable<string> SubCommandIdentities => Definition.SubCommandIdentities;

    /// <inheritdoc />
    public Func<TModel, TResult>? Handler => Definition.Handler;

    /// <inheritdoc />
    public override string ToString() => this.GetPathString();
}