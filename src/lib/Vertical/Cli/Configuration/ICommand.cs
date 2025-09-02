using Vertical.Cli.Help;
using Vertical.Cli.Invocation;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents a command.
/// </summary>
public interface ICommand
{
    /// <summary>
    /// Gets the name of the command.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets the sub commands of this instance.
    /// </summary>
    IReadOnlyList<ISubCommand> Commands { get; }
    
    /// <summary>
    /// Gets a space separated path.
    /// </summary>
    string Path { get; }
    
    /// <summary>
    /// Gets the application defined help tag.
    /// </summary>
    CommandHelpTag? HelpTag { get; }
    
    /// <summary>
    /// Gets whether the command is an invocation target.
    /// </summary>
    bool IsInvocationTarget { get; }

    /// <summary>
    /// Creates a handler request builder.
    /// </summary>
    /// <param name="configuration">The root configuration.</param>
    /// <param name="modelConfigurationFactory">The service that creates model configurations.</param>
    /// <returns><see cref="HandlerContextBuilder"/></returns>
    HandlerContextBuilder CreateRequestBuilder(
        IRootConfiguration configuration,
        IModelConfigurationFactory modelConfigurationFactory);
}