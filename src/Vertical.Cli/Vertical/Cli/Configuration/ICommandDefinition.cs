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
    IEnumerable<ICommandDefinition> GetChildDefinitions();
    
    /// <summary>
    /// Gets options that control command line processing.
    /// </summary>
    CliOptions Options { get; }
}