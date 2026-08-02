namespace Vertical.Cli.Help;

public sealed class CommandExtendedRemarks
{
    public CommandExtendedRemarks(string title, string remarks)
    {
        Title = title;
        Remarks = remarks;
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(remarks);
    }

    public string Title { get; }

    public string Remarks { get; }
}