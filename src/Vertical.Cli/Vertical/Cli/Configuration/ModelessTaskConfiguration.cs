namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents teh configuration of a short task.
/// </summary>
public abstract class ModelessTaskConfiguration : ICliSymbol
{
    internal ModelessTaskConfiguration(
        string[] names,
        string? description,
        CliScope scope)
    {
        Names = names;
        Description = description;
        Scope = scope;
    }

    /// <summary>
    /// Gets the names the task can be identified by.
    /// </summary>
    public string[] Names { get; }

    /// <inheritdoc />
    public string PrimaryIdentifier => Names[0];

    /// <summary>
    /// Gets a description of the task.
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// Gets the scope.
    /// </summary>
    public CliScope Scope { get; }

    /// <summary>
    /// Gets the task implementation.
    /// </summary>
    /// <returns>Task&lt;int&gt;</returns>
    /// <param name="command">Target command</param>
    /// <param name="options">Options</param>
    public abstract Task<int> InvokeAsync(CliCommand command, CliOptions options);
}