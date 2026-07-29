namespace Vertical.Cli.Diagnostics;

/// <summary>
/// Represents an error with a response argument.
/// </summary>
public sealed class ResponseResourceError : CommandLineError
{
    internal ResponseResourceError(string annotation, Exception exception) : base(FormatMessage(annotation, exception))
    {
        Annotation = annotation;
        Exception = exception;
    }

    /// <summary>
    /// Gets the annotation that references a file or resource that could not be loaded.
    /// </summary>
    public string Annotation { get; }
    
    /// <summary>
    /// Gets the exception that occurred.
    /// </summary>
    public Exception Exception { get; }

    private static string FormatMessage(string annotation, Exception exception)
    {
        return $"Could not load file or resource '{annotation}': {exception.Message}";
    }
}