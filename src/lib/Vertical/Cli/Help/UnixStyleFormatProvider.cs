namespace Vertical.Cli.Help;

/// <summary>
/// Implements a <see cref="IHelpProvider"/> that composes the help content in compact format.
/// </summary>
internal class UnixStyleFormatProvider(HelpFormattingOptions formattingOptions,
    Func<TextWriter> textWriterFactory,
    int renderWidth) : AbstractFormatProvider(formattingOptions, textWriterFactory, renderWidth)
{
    public static HelpFormattingOptions FormattingOptions => new()
    {
        SectionTitleFormatter = section => HelpFormattingOptions.Default.SectionTitleFormatter(section).ToUpper()
    };
    
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
                .WriteLine(HelpElement.OperandSyntax, item.OperandSyntax);

            if (item.Description.Length == 0)
            {
                layoutWriter.EnqueueBreak();
                continue;
            }
            
            layoutWriter
                .Indent(2)
                .WriteAnchored(HelpElement.Description, item.Description)
                .WriteLine()
                .EnqueueBreak();
        }

        layoutWriter.EnqueueBreak();
    }
}