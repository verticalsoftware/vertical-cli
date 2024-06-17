namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents the root-most command of an application.
/// </summary>
/// <typeparam name="TModel">Model type</typeparam>
/// <typeparam name="TResult">Result type</typeparam>
public sealed class RootCommand<TModel, TResult> : CliCommand<TModel, TResult>, IOptionsRoot 
    where TModel : class
{
    /// <inheritdoc />
    public RootCommand(
        string name,
        string? description = null) 
        : base([name], description, null)
    {
    }

    /// <summary>
    /// Gets the global options.
    /// </summary>
    public CliOptions Options { get; } = new();

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