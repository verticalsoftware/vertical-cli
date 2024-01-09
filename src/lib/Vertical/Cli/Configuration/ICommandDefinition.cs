using System.Diagnostics.CodeAnalysis;
using Vertical.Cli.Conversion;
using Vertical.Cli.Validation;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents the basic definition of a CLI command.
/// </summary>
public interface ICommandDefinition
{
    /// <summary>
    /// Gets the command this instance is defined in.
    /// </summary>
    ICommandDefinition? Parent { get; }
    
    /// <summary>
    /// Gets the command identity.
    /// </summary>
    string Id { get; }
    
    /// <summary>
    /// Gets the model type.
    /// </summary>
    Type ModelType { get; }
    
    /// <summary>
    /// Gets the result type.
    /// </summary>
    Type ResultType { get; }
    
    /// <summary>
    /// Gets the defined symbols.
    /// </summary>
    IReadOnlyCollection<SymbolDefinition> Symbols { get; }
    
    /// <summary>
    /// Gets the collection of <see cref="ValueConverter"/> instances.
    /// </summary>
    IReadOnlyCollection<ValueConverter> Converters { get; }
    
    /// <summary>
    /// Gets the collection of <see cref="Validator"/> instances.
    /// </summary>
    IReadOnlyCollection<Validator> Validators { get; }
    
    /// <summary>
    /// Gets whether the handler was assigned.
    /// </summary>
    bool HasHandler { get; }
    
    /// <summary>
    /// Gets the description to display for the command.
    /// </summary>
    string? Description { get; }
    
    /// <summary>
    /// Enumerates the identities of sub commands.
    /// </summary>
    IEnumerable<string> SubCommandIdentities { get; }

    /// <summary>
    /// Creates child command definitions.
    /// </summary>
    /// <returns>Enumeration of <see cref="ICommandDefinition"/></returns>
    IEnumerable<ICommandDefinition> CreateChildDefinitions();
    
    /// <summary>
    /// Gets options that control command line processing.
    /// </summary>
    CliOptions Options { get; }
}

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
}

/// <summary>
/// Represents the definition of a CLI command.
/// </summary>
/// <typeparam name="TModel">The type of model used by the handler implementation.</typeparam>
/// <typeparam name="TResult">The type of result produced by the command handler.</typeparam>
public interface ICommandDefinition<in TModel, TResult> : ICommandDefinition<TResult>
    where TModel : class
{
    /// <summary>
    /// Gets the function that implements the application's logic for this command.
    /// </summary>
    Func<TModel, TResult>? Handler { get; }
}