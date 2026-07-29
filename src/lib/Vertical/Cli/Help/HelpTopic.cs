namespace Vertical.Cli.Help;

public abstract class HelpTopic
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HelpTopic"/> class.
    /// </summary>
    /// <param name="remarks">The help content to display.</param>
    protected HelpTopic(string remarks)
    {
        Remarks = remarks;
    }

    /// <summary>
    /// Gets the help content to display.
    /// </summary>
    public string Remarks { get; }

    /// <inheritdoc />
    public override string ToString() => Remarks;
}