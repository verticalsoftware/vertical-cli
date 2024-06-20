using Vertical.Cli.Configuration;

namespace Vertical.Cli;

/// <summary>
/// Represents an error during command invocation.
/// </summary>
public sealed class CommandLineException : Exception
{
    internal CommandLineException(
        CommandLineError error,
        string message,
        string path,
        CliCommand command,
        CliSymbol? symbol = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        Error = error;
        Path = path;
        Command = command;
        Symbol = symbol;
    }

    /// <summary>
    /// Gets the error category.
    /// </summary>
    public CommandLineError Error { get; }

    /// <summary>
    /// Gets the path.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Gets the command.
    /// </summary>
    public CliCommand Command { get; }

    /// <summary>
    /// Gets the affected symbol, if available.
    /// </summary>
    public CliSymbol? Symbol { get; }
}