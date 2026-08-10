using Vertical.Cli.Configuration;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Help;

/// <summary>
/// Represents the default help provider.
/// </summary>
public class DefaultHelpProvider : IHelpProvider
{
    /// <inheritdoc />
    public virtual string? GetRemarks(IHelpSubject subject) => subject.GetRemarks();

    /// <inheritdoc />
    public virtual IEnumerable<ExtendedRemarksSection> GetExtendedRemarks(Command command) =>
        command.GetExtendedRemarksSections();

    /// <inheritdoc />
    public virtual string GetIdentifier(IHelpSubject subject) => subject.GetListIdentifier();

    /// <inheritdoc />
    public virtual string? GetParameterName(ICliSymbol subject) => subject.GetParameterName();
}