using Vertical.Cli.Configuration;
using Vertical.Cli.IO;

namespace Vertical.Cli.Help;

internal class OptionSymbolElement : IListElement
{
    private OptionSymbolElement(
        string arityAnnotation,
        string identifierList,
        string leftParameterEnclosure,
        string rightParameterEnclosure,
        string? parameterName,
        string aritySyntax,
        string remarks,
        int computedWidth)
    {
        Remarks = remarks;
        ComputedWidth = computedWidth;

        LeftParameterEnclosure = leftParameterEnclosure;
        RightParameterEnclosure = rightParameterEnclosure;
        ArityAnnotation = arityAnnotation;
        IdentifierList = identifierList;
        ParameterName = parameterName ?? " ";
        AritySyntax = aritySyntax;
    }

    public string ArityAnnotation { get; set; }

    public string IdentifierList { get; set; }

    public string LeftParameterEnclosure { get; set; }

    public string RightParameterEnclosure { get; set; }

    public string ParameterName { get; set; }

    public string AritySyntax { get; set; }

    /// <inheritdoc />
    public string Remarks { get; }

    /// <inheritdoc />
    public int ComputedWidth { get; }

    /// <inheritdoc />
    public void RenderSyntax(OutputWriter writer)
    {
        writer.Write(ArityAnnotation, DisplayElement.Important);
        writer.Write(IdentifierList, DisplayElement.ListIdentifier);
        writer.Write(LeftParameterEnclosure, DisplayElement.ParameterSyntax);
        writer.Write(ParameterName, DisplayElement.ParameterSyntax);
        writer.Write(RightParameterEnclosure, DisplayElement.ParameterSyntax);
        writer.Write(AritySyntax, DisplayElement.ParameterSyntax);
    }

    public static OptionSymbolElement Create(IHelpProvider provider, ICliSymbol option)
    {
        var required = option.Arity.Minimum > 0;
        var arityAnnotation = required
            ? "* "
            : "  ";
        var identifier = provider.GetIdentifier(option);
        var parameterName = provider.GetParameterName(option);
        var (leftEnclosure, rightEnclosure) = option is CliSymbol { Kind: SymbolKind.Option }
            ? (" <", ">")
            : (string.Empty, string.Empty);
        var aritySyntax = option.Arity.Maximum.GetValueOrDefault(2) > 1
            ? "..."
            : string.Empty;
        var remarks = provider.GetRemarks(option) ?? string.Empty;
        var computedLength = arityAnnotation.Length +
                             identifier.Length +
                             leftEnclosure.Length +
                             (parameterName?.Length ?? 0) +
                             rightEnclosure.Length +
                             aritySyntax.Length;

        return new OptionSymbolElement(arityAnnotation,
            identifier,
            leftEnclosure,
            rightEnclosure,
            parameterName,
            aritySyntax,
            remarks,
            computedLength);
    }
}