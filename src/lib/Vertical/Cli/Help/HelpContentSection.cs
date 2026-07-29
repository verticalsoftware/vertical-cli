namespace Vertical.Cli.Help;

/// <summary>
/// Represents a section displayed at the end of a help article. 
/// </summary>
public sealed class HelpContentSection
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HelpContentSection"/> class.
    /// </summary>
    /// <param name="heading">The section title.</param>
    /// <param name="remarks">Section help content.</param>
    public HelpContentSection(string heading, string remarks)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(heading);
        ArgumentException.ThrowIfNullOrWhiteSpace(remarks);
        
        Heading = heading;
        Remarks = remarks;
    }

    /// <summary>
    /// GEts the section title.
    /// </summary>
    public string Heading { get; }

    /// <summary>
    /// Gets the content.
    /// </summary>
    public string Remarks { get; }

    /// <inheritdoc />
    public override string ToString() => Heading;
}