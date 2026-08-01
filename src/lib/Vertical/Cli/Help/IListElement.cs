using Vertical.Cli.IO;

namespace Vertical.Cli.Help;

public interface IListElement
{
    string Remarks { get; }
    int ComputedWidth { get; }
    void RenderSyntax(OutputWriter writer);
}