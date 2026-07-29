using Vertical.Cli.IO;

namespace Vertical.Cli.Diagnostics;

/// <summary>
/// Represents a collection of errors.
/// </summary>
public class AggregateCommandLineError : CommandLineError
{
    internal AggregateCommandLineError(IReadOnlyCollection<CommandLineError> errors) : base(FormatMessage(errors))
    {
        Errors = errors;
    }

    /// <summary>
    /// Gets the collection of errors.
    /// </summary>
    public IReadOnlyCollection<CommandLineError> Errors { get; }

    /// <inheritdoc />
    public override void WriteOutputMessage(OutputWriter writer)
    {
        writer.SetDisplayElement(DisplayElement.Important);
        
        foreach (var error in Errors)
        {
            error.WriteOutputMessage(writer);    
        }
    }

    private static string FormatMessage(IReadOnlyCollection<CommandLineError> errors)
    {
        return $"One or more error(s) found.";
    }
}