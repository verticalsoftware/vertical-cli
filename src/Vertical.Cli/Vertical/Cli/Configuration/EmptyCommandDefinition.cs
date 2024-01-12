using System.Diagnostics.CodeAnalysis;
using Vertical.Cli.Invocation;

namespace Vertical.Cli.Configuration;

internal sealed class EmptyCommandDefinition<TResult> : ICommandDefinition<None, TResult>
{
    internal EmptyCommandDefinition(CliOptions options)
    {
        Options = options;
    }
    
    /// <inheritdoc />
    public ICommandDefinition? Parent => null;

    /// <inheritdoc />
    public string Id => string.Empty;

    /// <inheritdoc />
    public Type ModelType => typeof(None);

    /// <inheritdoc />
    public Type ResultType => typeof(TResult);

    /// <inheritdoc />
    public IReadOnlyCollection<SymbolDefinition> Symbols => Array.Empty<SymbolDefinition>();

    /// <inheritdoc />
    public bool HasHandler => true;

    /// <inheritdoc />
    public string? Description => null;

    /// <inheritdoc />
    public IEnumerable<string> SubCommandIdentities => Enumerable.Empty<string>();

    /// <inheritdoc />
    public IEnumerable<ICommandDefinition> GetChildDefinitions() => Enumerable.Empty<ICommandDefinition>();

    /// <inheritdoc />
    public CliOptions Options { get; }

    /// <inheritdoc />
    public bool TryCreateChild(string id, [NotNullWhen(true)] out ICommandDefinition<TResult>? child)
    {
        child = null;
        return false;
    }

    /// <inheritdoc />
    public IEnumerable<ICommandDefinition<TResult>> SubCommands => Enumerable.Empty<ICommandDefinition<TResult>>();

    /// <inheritdoc />
    public ICallSite<TResult> CreateCallSite() => throw new InvalidOperationException();

    /// <inheritdoc />
    public Func<None, CancellationToken, TResult>? Handler => null;
}