using Vertical.Cli.Configuration;
using Vertical.Cli.IO;

namespace Vertical.Cli.Help;

internal sealed class DirectiveSymbolElement : IListElement
{
    private DirectiveSymbolElement(string identifier, string parameterSyntax, string remarks, int computedLength)
    {
        Remarks = remarks;
        ComputedWidth = computedLength;
        
        Identifier = identifier;
        ParameterSyntax = parameterSyntax;
    }

    public string Identifier { get; set; }

    public string ParameterSyntax { get; set; }

    /// <inheritdoc />
    public string Remarks { get; }

    /// <inheritdoc />
    public int ComputedWidth { get; }

    /// <inheritdoc />
    public void RenderSyntax(OutputWriter writer)
    {
        writer.Write('[', DisplayElement.ListIdentifier);
        writer.Write(Identifier, DisplayElement.ListIdentifier);
        writer.Write(ParameterSyntax, DisplayElement.ParameterSyntax);
        writer.Write(']', DisplayElement.ListIdentifier);
    }

    public static DirectiveSymbolElement Create(IHelpProvider provider, ICliSymbol directive)
    {
        var identifier = provider.GetIdentifier(directive);
        var parameterName = provider.GetParameterName(directive);
        var parameterSyntax = directive.ParameterArity switch
        {
            null => string.Empty,
            ParameterArity.ZeroOrOne => $"[=<{parameterName}>]",
            _ => $"=<{parameterName}>"
        };
        var computedLength = identifier.Length +
                             parameterSyntax.Length +
                             2; // enclosures

        var remarks = provider.GetRemarks(directive) ?? string.Empty;

        return new DirectiveSymbolElement(
            identifier,
            parameterSyntax,
            remarks,
            computedLength);
    }
}