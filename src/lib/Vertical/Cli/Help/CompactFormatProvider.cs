namespace Vertical.Cli.Help;

/// <summary>
/// Implements a <see cref="IHelpProvider"/> that composes the help content in compact format.
/// </summary>
internal class CompactFormatProvider(HelpFormattingOptions formatterOptions, 
    Func<TextWriter> textWriterFactory, 
    int renderWidth) 
    : AbstractFormatProvider(formatterOptions, textWriterFactory, renderWidth)
{
    protected override void WriteParameterGroup(HelpLayoutWriter layoutWriter, 
        string key,
        IEnumerable<DisplayParameter> group, 
        int padding)
    {
        layoutWriter.WriteLine(HelpElement.SectionTitle, key);

        foreach (var item in group)
        {
            layoutWriter
                .Indent()
                .Write(HelpElement.IdentifierList, item.IdentifierSyntax)
                .EnqueueSpace()
                .Write(HelpElement.OperandSyntax, item.OperandSyntax)
                .IndentTo(padding)
                .WriteAnchored(HelpElement.Description, item.Description)
                .WriteLine();
        }

        layoutWriter.EnqueueBreak();
    }
}