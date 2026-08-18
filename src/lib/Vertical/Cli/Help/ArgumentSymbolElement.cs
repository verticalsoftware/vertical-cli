using Vertical.Cli.Configuration;
using Vertical.Cli.IO;

namespace Vertical.Cli.Help;

internal sealed class ArgumentSymbolElement : IListElement
{
    private ArgumentSymbolElement(
        string arityAnnotation,
        string identifier,
        char leftEnclosure,
        char rightEnclosure,
        string aritySyntax,
        string remarks,
        int computedLength)
    {
        ArityAnnotation = arityAnnotation;
        Identifier = identifier;
        LeftEnclosure = leftEnclosure;
        RightEnclosure = rightEnclosure;
        AritySyntax = aritySyntax;
        Remarks = remarks;
        ComputedWidth = computedLength;
    }

    public string ArityAnnotation { get; }
    public string Identifier { get; }
    public char LeftEnclosure { get; }
    public char RightEnclosure { get; }
    public string AritySyntax { get; }
    public string Remarks { get; }
    
    public int ComputedWidth { get; }

    public static ArgumentSymbolElement Create(IHelpProvider provider, CliSymbol argument)
    {
        var required = argument.Arity.Minimum > 0;

        var arityAnnotation = required ? "* " : "  ";
        var (leftEnclosure, rightEnclosure) = ('<', '>');
        var identifier = provider.GetIdentifier(argument);
        var aritySyntax = argument.Arity.Maximum.GetValueOrDefault(2) > 1
            ? "..."
            : string.Empty;
        var computedLength = arityAnnotation.Length +
                             2 +
                             identifier.Length +
                             aritySyntax.Length;

        var remarks = provider.GetRemarks(argument);
        
        return new ArgumentSymbolElement(
            arityAnnotation, 
            identifier, 
            leftEnclosure, 
            rightEnclosure,
            aritySyntax,
            remarks ?? string.Empty,
            computedLength);
    }

    public void RenderSyntax(OutputWriter writer)
    {
        writer.Write(ArityAnnotation, DisplayElement.Important);
        writer.Write(LeftEnclosure, DisplayElement.ListIdentifier);
        writer.Write(Identifier, DisplayElement.ListIdentifier);
        writer.Write(RightEnclosure, DisplayElement.ListIdentifier);
        writer.Write(AritySyntax, DisplayElement.ParameterSyntax);
    }

    public void RenderParameterSyntax(OutputWriter writer)
    {
        writer.Write(LeftEnclosure, DisplayElement.ParameterSyntax);
        writer.Write(Identifier, DisplayElement.ParameterSyntax);
        writer.Write(RightEnclosure, DisplayElement.ParameterSyntax);
        writer.Write(AritySyntax, DisplayElement.ParameterSyntax);
    }
}