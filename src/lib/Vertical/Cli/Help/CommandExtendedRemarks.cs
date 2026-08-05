namespace Vertical.Cli.Help;

/// <summary>
/// Represents an extended remarks section for a command.
/// </summary>
public sealed class CommandExtendedRemarks
{
    /// <summary>
    /// Initializezs a new instance of the <see cref="CommandExtendedRemarks"/> class.
    /// </summary>
    /// <param name="title">The section title.</param>
    /// <param name="remarks">Remarks to include in the section.</param>
    public CommandExtendedRemarks(string title, string remarks)
    {
        Title = title;
        Remarks = remarks;
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(remarks);
    }

    /// <summary>
    /// Gets the title of the extended section.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets the remarks of the extended section.
    /// </summary>
    public string Remarks { get; }
}