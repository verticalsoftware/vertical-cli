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
    public virtual IEnumerable<CommandExtendedRemarks> GetExtendedRemarks(Command command)
    {
        return command.HelpTopic?.ExtendedRemarks ?? [];
    }

    /// <inheritdoc />
    public virtual string GetListIdentifier(IHelpSubject subject)
    {
        return subject switch
        {
            CliSymbol { Kind: SymbolKind.PositionArgument } argument  => 
                argument.HelpTopic?.ParameterSyntax ?? argument.BindingName.ToKebabCase(),
            CliSymbol { Kind: SymbolKind.Option or SymbolKind.Switch } named => 
                string.Join(", ", named.Aliases),
            HelpOptionSymbol helpSymbol => string.Join(", ", helpSymbol.Aliases),
            IDirectiveSymbol directive => directive.Identifier,
            _ => throw new NotSupportedException()
        };
    }

    /// <inheritdoc />
    public virtual string? GetParameterName(ICliSymbol subject)
    {
        return subject switch
        {
            CliSymbol { Kind: SymbolKind.PositionArgument } symbol => GetListIdentifier(symbol),
            CliSymbol { Kind: SymbolKind.Option } option => option.HelpTopic?.ParameterSyntax ??
                                                                  option.BindingName.ToKebabCase(),
            CliSymbol { Kind: SymbolKind.Switch } => string.Empty,
            IDirectiveSymbol { ParameterArity: not null, HelpTopic: SymbolHelpTopic topic } => topic.ParameterSyntax ?? "value",
            IDirectiveSymbol => string.Empty,
            HelpOptionSymbol => string.Empty,
            _ => null
        };
    }
}