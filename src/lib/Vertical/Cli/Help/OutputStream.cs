namespace Vertical.Cli.Help;

/// <summary>
/// Defines the virtual device help output is being sent to.
/// </summary>
/// <param name="TextWriter">The <see cref="TextWriter"/> that will receive output.</param>
/// <param name="DisplayWidth">The maximum number of character to print on each line of text.</param>
public readonly record struct OutputStream(TextWriter TextWriter, int DisplayWidth);