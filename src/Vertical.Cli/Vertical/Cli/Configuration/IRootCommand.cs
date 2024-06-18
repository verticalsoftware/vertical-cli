namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents the root command.
/// </summary>
public interface IRootCommand
{
    /// <summary>
    /// Gets the root options.
    /// </summary>
    CliOptions Options { get; }
}