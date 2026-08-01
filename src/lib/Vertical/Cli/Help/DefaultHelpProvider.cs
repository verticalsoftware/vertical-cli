using Vertical.Cli.Configuration;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Help;

/// <summary>
/// Represents the default help provider.
/// </summary>
public class DefaultHelpProvider : IHelpProvider
{
    /// <inheritdoc />
    public virtual string? GetRemarks(IHelpSubject subject) => subject.HelpTopic?.Remarks;

    /// <inheritdoc />
    public int GetCommandSectionsCount(Command command)
    {
        return command.HelpTopic?.SectionContent?.Length ?? 0;
    }

    /// <inheritdoc />
    public string GetCommandSectionHeading(Command command, int sectionId)
    {
        return command.HelpTopic?.SectionContent is { } sectionContent
               && sectionId < sectionContent.Length
            ? sectionContent[sectionId].Heading
            : throw new InvalidOperationException($"Command '{command.Path}' does not have section {sectionId}.");
    }

    /// <inheritdoc />
    public string GetCommandSectionRemarks(Command command, int sectionId)
    {
        return command.HelpTopic?.SectionContent is { } sectionContent
               && sectionId < sectionContent.Length
            ? sectionContent[sectionId].Remarks
            : throw new InvalidOperationException($"Command '{command.Path}' does not have section {sectionId}.");
    }

    /// <inheritdoc />
    public string GetListIdentifier(IHelpSubject subject)
    {
        return subject switch
        {
            CliSymbol { Kind: SymbolKind.PositionArgument } argument  => 
                argument.HelpTopic?.ParameterSyntax ?? argument.BindingName.ToKebabCase(),
            CliSymbol { Kind: SymbolKind.Option or SymbolKind.Switch } named => 
                string.Join(", ", named.Aliases),
            IDirectiveSymbol directive => directive.Identifier,
            _ => throw new NotSupportedException()
        };
    }

    /// <inheritdoc />
    public string GetParameterName(IHelpSubject subject)
    {
        return subject switch
        {
            CliSymbol { Kind: SymbolKind.PositionArgument } symbol => GetListIdentifier(symbol),
            CliSymbol { Kind: SymbolKind.Option } option => option.HelpTopic?.ParameterSyntax ??
                                                                  option.BindingName.ToKebabCase(),
            CliSymbol { Kind: SymbolKind.Switch } => string.Empty,
            IDirectiveSymbol { HelpTopic: SymbolHelpTopic topic } => topic.ParameterSyntax ?? "value",
            IDirectiveSymbol => "value",
            _ => throw new NotSupportedException()
        };
    }
}