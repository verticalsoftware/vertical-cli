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
    public const string Id = "(root)";

    /// <summary>
    /// Creates a root command that delegates application logic handling to sub commands.
    /// </summary>
    /// <param name="id">The name of the application.</param>
    /// <param name="configure">A delegate that presents a fluent builder that is used to configure the
    /// command definition.</param>
    /// <param name="options">Options that control command line processing at the global level.</param>
    /// <typeparam name="TResult">The type of result produced by the command handler.</typeparam>
    /// <returns>The root command instance.</returns>
    public static IRootCommand<None, TResult> Create<TResult>(
        string id,
        Action<ICommandBuilder<None, TResult>> configure,
        CliOptions? options = null)
    {
        return Create<None, TResult>(id, configure, options);
    }

    /// <summary>
    /// Creates a root command.
    /// </summary>
    /// <param name="id">The name of the application.</param>
    /// <param name="configure">
    ///     A delegate that presents a fluent builder that is used to configure the command definition.
    /// </param>
    /// <param name="options">Options that control command line processing at the global level.</param>
    /// <typeparam name="TModel">The type of model used by the handler implementation.</typeparam>
    /// <typeparam name="TResult">The type of result produced by the command handler.</typeparam>
    /// <returns>The root command instance.</returns>
    public static IRootCommand<TModel, TResult> Create<TModel, TResult>(
        string id,
        Action<ICommandBuilder<TModel, TResult>> configure,
        CliOptions? options = null)
        where TModel : class
    {
        Guard.IsNotNull(id);
        Guard.IsNotNull(configure);

        var builder = new CommandBuilder<TModel, TResult>(id, options ?? new CliOptions());
        configure(builder);
        
        return new RootCommandDefinition<TModel, TResult>(builder);
    }
}