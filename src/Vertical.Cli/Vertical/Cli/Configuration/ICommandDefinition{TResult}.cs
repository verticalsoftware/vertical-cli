using System.Diagnostics.CodeAnalysis;
using Vertical.Cli.Invocation;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents the definition of a CLI command.
/// </summary>
/// <typeparam name="TResult">The type of result produced by the command handler.</typeparam>
public interface ICommandDefinition<TResult> : ICommandDefinition
{
    /// <summary>
    /// Creates a <see cref="ICommandDefinition{TResult}"/> given the id of a sub command definition in this
    /// instance.
    /// </summary>
    /// <param name="id">The sub command identity.</param>
    /// <param name="child">If <paramref name="id"/> is found, the command definition instance.</param>
    /// <returns>A <c>bool</c> indicating whether the definition was created.</returns>
    bool TryCreateChild(string id, [NotNullWhen(true)] out ICommandDefinition<TResult>? child);
    
    /// <summary>
    /// Enumerates the defined sub commands.
    /// </summary>
    IEnumerable<ICommandDefinition<TResult>> SubCommands { get; }

    /// <summary>
    /// Creates a call site to this command.
    /// </summary>
    /// <returns><see cref="ICallSite{TResult}"/></returns>
    ICallSite<TResult> CreateCallSite();
}
