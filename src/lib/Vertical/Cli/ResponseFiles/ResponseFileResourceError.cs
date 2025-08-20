using Vertical.Cli.Invocation;

namespace Vertical.Cli.ResponseFiles;

/// <summary>
/// Represents an error that occurred when accessing a response resource.
/// </summary>
public sealed class ResponseFileResourceError : UsageError
{
    internal ResponseFileResourceError(string resource, Exception exception)
    {
        Resource = resource;
        Exception = exception;
    }

    /// <summary>
    /// Gets the resource that failed.
    /// </summary>
    public string Resource { get; }

    /// <summary>
    /// Gets the exception that occurred.
    /// </summary>
    public Exception Exception { get; }

    /// <inheritdoc />
    public override void WriteMessages(TextWriter textWriter)
    {
        textWriter.WriteLine($"Failed to access response resource '{Resource}':");
        textWriter.WriteLine($"  -> {Exception.Message}");
    }
}