namespace Vertical.Cli.IO;

/// <summary>
/// Represents an abstraction to the output device.
/// </summary>
public interface IConsole
{
    /// <summary>
    /// Gets the input text reader for this instance.
    /// </summary>
    TextReader In { get; }
    
    /// <summary>
    /// Gets the output text writer for this instance.
    /// </summary>
    TextWriter Out { get; }
    
    /// <summary>
    /// Gets the number of characters that can be printed before line wrapping occurs.
    /// </summary>
    int DisplayWidth { get; }
    
    /// <summary>
    /// Gets/sets whether to style the output as error text.
    /// </summary>
    bool ErrorMode { get; set; }

    /// <summary>
    /// Notify the given action about a cancel event.
    /// </summary>
    /// <param name="handleCancel">The action to receive the event and return whether the cancellation is
    /// accepted.</param>
    void HandleCancelEvent(Action handleCancel);

    /// <summary>
    /// Writes text to the underlying text writer.
    /// </summary>
    /// <param name="str">The string to write.</param>
    void Write(string str);
    
    /// <summary>
    /// Writes a new line character to the underlying text writer.
    /// </summary>
    void WriteLine();
    
    /// <summary>
    /// Writes text and a new line character to the underlying text writer.
    /// </summary>
    /// <param name="str">The string to write.</param>
    void WriteLine(string str);

    /// <summary>
    /// Sets error output mode to <c>true</c>, writes the given text to the underlying text writer, then restores the
    /// output mode to its previous setting.
    /// </summary>
    /// <param name="str">The string to write.</param>
    void WriteErrorLine(string str);
}