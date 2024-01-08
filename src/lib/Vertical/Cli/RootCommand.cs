using CommunityToolkit.Diagnostics;
using Vertical.Cli.Configuration;

namespace Vertical.Cli;

/// <summary>
/// Provides a set of static methods used to configure a root command.
/// </summary>
public static class RootCommand
{
    /// <summary>
    /// Defines the root command id.
    /// </summary>
    public static string Id { get; } = "(root)";
    
    /// <summary>
    /// Creates a root command that delegates application logic handling to sub commands.
    /// </summary>
    /// <param name="configure">
    /// A delegate that presents a fluent builder that is used to configure the command definition.
    /// </param>
    /// <typeparam name="TResult">The type of result produced by the command handler.</typeparam>
    /// <returns>The root command instance.</returns>
    public static IRootCommand<None, TResult> Create<TResult>(
        Action<ICommandBuilder<None, TResult>> configure)
    {
        return Create<None, TResult>(configure);
    }
    
    /// <summary>
    /// Creates a root command.
    /// </summary>
    /// <param name="configure">
    /// A delegate that presents a fluent builder that is used to configure the command definition.
    /// </param>
    /// <typeparam name="TModel">The type of model used by the handler implementation.</typeparam>
    /// <typeparam name="TResult">The type of result produced by the command handler.</typeparam>
    /// <returns>The root command instance.</returns>
    public static IRootCommand<TModel, TResult> Create<TModel, TResult>(
        Action<ICommandBuilder<TModel, TResult>> configure)
        where TModel : class
    {
        Guard.IsNotNull(configure);

        var builder = new CommandBuilder<TModel, TResult>(Id);
        configure(builder);
        
        return new RootCommandDefinition<TModel, TResult>(builder);
    }
}