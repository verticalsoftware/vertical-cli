using System.Diagnostics.CodeAnalysis;
using Vertical.Cli.Invocation;
using Vertical.Cli.Utilities;

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

    private ICommandDefinition<TModel, TResult> Definition { get; }

    /// <inheritdoc />
    public string Id => Definition.Id;

    /// <inheritdoc />
    public Type ModelType => Definition.ModelType;

    /// <inheritdoc />
    public Type ResultType => Definition.ResultType;

    /// <inheritdoc />
    public IReadOnlyCollection<SymbolDefinition> Symbols => Definition.Symbols;

    /// <inheritdoc />
    public bool HasHandler => Definition.HasHandler;

    /// <inheritdoc />
    public string? Description => Definition.Description;

    /// <inheritdoc />
    public ICommandDefinition? Parent => Definition.Parent;

    /// <inheritdoc />
    public bool TryCreateChild(string id, [NotNullWhen(true)] out ICommandDefinition<TResult>? child) => 
        Definition.TryCreateChild(id, out child);

    /// <inheritdoc />
    public IEnumerable<ICommandDefinition<TResult>> SubCommands => Definition.SubCommands;

    /// <inheritdoc />
    public ICallSite<TResult> CreateCallSite()
    {
        var handler = Handler ?? throw new InvalidOperationException();
        return CallSite<TResult>.Create(this, handler, CallState.Command);
    }
    
    /// <inheritdoc />
    public IEnumerable<string> SubCommandIdentities => Definition.SubCommandIdentities;

    /// <inheritdoc />
    public IEnumerable<ICommandDefinition> GetChildDefinitions() => Definition.GetChildDefinitions();

    /// <inheritdoc />
    public Func<TModel, CancellationToken, TResult>? Handler => Definition.Handler;

    /// <inheritdoc />
    public override string ToString() => this.GetPathString();
}