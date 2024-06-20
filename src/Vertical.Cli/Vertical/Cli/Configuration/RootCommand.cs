using Vertical.Cli.Configuration;

// ReSharper disable once CheckNamespace
namespace Vertical.Cli;

/// <summary>
/// Represents the root-most command of an application.
/// </summary>
/// <typeparam name="TModel">Model type</typeparam>
public sealed class RootCommand<TModel> : 
    CliCommand<TModel>, IRootCommand
    where TModel : class
{
    /// <inheritdoc />
    public RootCommand(
        string name,
        string? description = null) 
        : base(Configuration.SymbolId.Root, [name], description, new SymbolId())
    {
    }

    /// <summary>
    /// Gets the options.
    /// </summary>
    public CliOptions Options { get; } = new();

    /// <summary>
    /// Adds help functionality.
    /// </summary>
    /// <param name="names">The names the switch can be identified by.</param>
    /// <param name="description">Description of the help option.</param>
    /// <param name="handler">Handler that displays the help content.</param>
    /// <param name="result">Result to return from the default handler.</param>
    /// <returns>A reference to this instance.</returns>
    public RootCommand<TModel> AddHelpSwitch(
        string[]? names = null,
        string? description = null,
        Func<CliCommand, CliOptions, Task<int>>? handler = null,
        int result = 0)
    {
        if (names is { Length: 0 })
        {
            throw new ArgumentException("Help option names cannot be empty.", nameof(names));
        }
        
        AddModelessTask(new HelpTaskConfiguration(
            SymbolId.Next(),
            names ?? ["--help", "?", "-?"],
            description ?? "Display help content",
            CliScope.SelfAndDescendants,
            result));

        return this;
    }

    /// <summary>
    /// Configure options fluently.
    /// </summary>
    /// <param name="configure">Action that configures the provided <see cref="CliOptions"/></param>
    /// <returns>A reference to this instance.</returns>
    public RootCommand<TModel> ConfigureOptions(Action<CliOptions> configure)
    {
        configure(Options);
        return this;
    }

    /// <summary>
    /// Performs verbose configuration checking.
    /// </summary>
    public void VerifyConfiguration()
    {
        var messages = new List<string>();
        VerifyConfiguration(messages);

        if (messages.Count == 0)
            return;

        throw new InvalidOperationException(string.Join(Environment.NewLine, messages));
    }
}