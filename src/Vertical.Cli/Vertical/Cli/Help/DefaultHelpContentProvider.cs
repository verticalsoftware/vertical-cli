using Vertical.Cli.Configuration;

namespace Vertical.Cli.Help;

internal sealed class DefaultHelpContentProvider : IHelpContentProvider
{
    /// <inheritdoc />
    public string? GetContent(CliPrimitive cliObject) => cliObject.Description;
}