using Vertical.Cli.Configuration;

namespace Vertical.Cli.Help;

internal sealed class DefaultHelpContentProvider : IHelpContentProvider
{
    /// <inheritdoc />
    public string? GetContent(CliObject cliObject) => cliObject.Description;
}