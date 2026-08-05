using System.Text;

namespace Vertical.Cli.Diagnostics;

/// <summary>
/// Represents an aggregation of command line errors.
/// </summary>
public sealed class CommandLineException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandLineException"/> class.
    /// </summary>
    /// <param name="errors">The errors to wrap into the exception.</param>
    public CommandLineException(IReadOnlyCollection<CommandLineError> errors)
        : base(FormatMessage(errors))
    {
        ArgumentNullException.ThrowIfNull(errors);
        Errors = errors;
    }

    /// <summary>
    /// Gets the errors.
    /// </summary>
    public IReadOnlyCollection<CommandLineError> Errors { get; }

    private static string FormatMessage(IReadOnlyCollection<CommandLineError> errors)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Command line errors found ({errors.Count}");

        _ = errors.Aggregate(sb, (builder, next) => builder.AppendLine($"  {next}"));
        return sb.ToString();
    }
}