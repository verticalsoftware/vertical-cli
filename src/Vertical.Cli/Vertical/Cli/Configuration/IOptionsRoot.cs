namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents an object that exposes <see cref="CliOptions"/>
/// </summary>
public interface IOptionsRoot
{
    /// <summary>
    /// Gets the root options.
    /// </summary>
    CliOptions Options { get; }
}