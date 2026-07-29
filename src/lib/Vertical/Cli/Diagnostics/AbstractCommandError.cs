using Vertical.Cli.Configuration;

namespace Vertical.Cli.Diagnostics;

/// <summary>
/// Indicates the selected target command does not implement an application function, nor
/// does it define any sub commands.
/// </summary>
public sealed class AbstractCommandError : CommandLineError
{
    /// <inheritdoc />
    public AbstractCommandError(Command target) : base("Sub command name not provided.")
    {
        Target = target;
    }

    /// <summary>
    /// Gets the command selected by application input.
    /// </summary>
    public Command Target { get; }
}