namespace Vertical.Cli.Invocation;

/// <summary>
/// Represents the base class for usage errors.
/// </summary>
public abstract class UsageError
{
    /// <summary>
    /// When implemented by a class, writes one or more output messages to the provided
    /// text writer.
    /// </summary>
    /// <param name="textWriter">The text writer to writer messages to.</param>
    public abstract void WriteMessages(TextWriter textWriter);

    /// <inheritdoc />
    public override string ToString()
    {
        using var stringWriter = new StringWriter();
        WriteMessages(stringWriter);

        return stringWriter.ToString();
    }
}