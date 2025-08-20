using Vertical.Cli.Invocation;

namespace Vertical.Cli.ResponseFiles;

/// <summary>
/// Represents an error detected in a response file.
/// </summary>
public sealed class ResponseFileParseError : UsageError
{
    internal ResponseFileParseError(string resource, 
        int linePosition, 
        int columnPosition, 
        string message)
    {
        Resource = resource;
        LinePosition = linePosition;
        ColumnPosition = columnPosition;
        Message = message;
    }

    /// <summary>
    /// Gets the name of the resource.
    /// </summary>
    public string Resource { get; }

    /// <summary>
    /// Gets the line position of the error.
    /// </summary>
    public int LinePosition { get; }

    /// <summary>
    /// Gets the column position of the error.
    /// </summary>
    public int ColumnPosition { get; }

    /// <summary>
    /// Gets the message.
    /// </summary>
    public string Message { get; }

    /// <inheritdoc />
    public override void WriteMessages(TextWriter textWriter)
    {
        textWriter.WriteLine($"Error in response resource '{Resource}', @{LinePosition}:{ColumnPosition}");
        textWriter.WriteLine($"  -> {Message}");
    }
}