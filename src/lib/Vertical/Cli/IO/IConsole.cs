namespace Vertical.Cli.IO;

/// <summary>
/// Abstracts input and output operations.
/// </summary>
public interface IConsole
{
    /// <summary>
    /// Gets the input text reader.
    /// </summary>
    TextReader In { get; }
    
    /// <summary>
    /// Gets the output text writer.
    /// </summary>
    TextWriter Out { get; }
    
    /// <summary>
    /// Gets whether std out is redirected.
    /// </summary>
    bool IsOutputRedirected { get; }
    
    /// <summary>
    /// Gets the number of characters displayable on one row of the output device.
    /// </summary>
    int DisplayWidth { get; }
}