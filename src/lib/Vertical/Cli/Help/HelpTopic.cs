namespace Vertical.Cli.Help;

/// <summary>
/// Represents a help topic.
/// </summary>
public class HelpTopic
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HelpTopic"/> class.
    /// </summary>
    /// <param name="remarks">The help content to display.</param>
    internal HelpTopic(string remarks)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(remarks);
        Remarks = remarks;
    }

    /// <summary>
    /// Gets the help content to display.
    /// </summary>
    public string Remarks { get; }

    /// <inheritdoc />
    public override string ToString() => Remarks;

    /// <summary>
    /// Implicitly creates a new <see cref="HelpTopic"/> instance from a string.
    /// </summary>
    public static implicit operator HelpTopic(string str) => new(str);
}