using Vertical.Cli.Configuration;
using Vertical.Cli.IO;

namespace Vertical.Cli.Help;

internal sealed class CommandSymbolElement : IListElement
{
    private CommandSymbolElement(string identifier, string? remarks)
    {
        Remarks = remarks ?? string.Empty;
        Identifier = identifier;
    }

    public static CommandSymbolElement Create(IHelpProvider provider, Command command)
    {
        var identifier = command.Name;
        var remarks = provider.GetRemarks(command);

        return new CommandSymbolElement(identifier, remarks);
    }

    public string Identifier { get; set; }

    /// <inheritdoc />
    public string Remarks { get; }

    /// <inheritdoc />
    public int ComputedWidth => Identifier.Length;

    /// <inheritdoc />
    public void RenderSyntax(OutputWriter writer)
    {
        writer.Write(Identifier, DisplayElement.ListIdentifier);
    }
}