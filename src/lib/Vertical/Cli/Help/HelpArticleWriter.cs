using Vertical.Cli.Configuration;
using Vertical.Cli.IO;

namespace Vertical.Cli.Help;

/// <summary>
/// Displays help to the console output.
/// </summary>
public class HelpArticleWriter
{
    private const int IndentSpaces = FormattingConstants.IndentSpaces;
    
    /// <summary>
    /// Writes help content to console output.
    /// </summary>
    /// <param name="eventInfo">An object that contains all the applicable subjects of the article.</param>
    public virtual void WriteContent(HelpEventInfo eventInfo)
    {
        WriteCommandDescription(eventInfo);
        WriteUsageSection(eventInfo);
        WriteSubCommandsSection(eventInfo);
        WriteArgumentsSection(eventInfo);
        WriteOptionsSection(eventInfo);
        WriteDirectivesSection(eventInfo);
    }

    private static void WriteArgumentsSection(HelpEventInfo eventInfo)
    {
        var symbols = eventInfo.PositionalSymbols;
        if (symbols.Count == 0) return;
        
        var writer = eventInfo.OutputWriter;
        var provider = eventInfo.HelpProvider;
        var lineBounds = new LineBounds(0, eventInfo.DisplayWidth);
        
        writer.WriteLine("Arguments:", DisplayElement.Heading);

        var elements = symbols.Select(symbol => ArgumentSymbolElement.Create(provider, symbol));
        writer.WriteTable(elements, lineBounds);
        writer.WriteLine();
    }

    private static void WriteOptionsSection(HelpEventInfo eventInfo)
    {
        var symbols = eventInfo.NamedSymbols;
        
        var writer = eventInfo.OutputWriter;
        var provider = eventInfo.HelpProvider;
        var lineBounds = new LineBounds(0, eventInfo.DisplayWidth);
        
        writer.WriteLine("Options:", DisplayElement.Heading);

        var elements = symbols
            .Select(symbol => OptionSymbolElement.Create(provider, symbol))
            .Concat(eventInfo.UnboundOptionSymbols.Select(symbol => OptionSymbolElement.Create(provider, symbol)));
        
        writer.WriteTable(elements, lineBounds);
        writer.WriteLine();
    }

    private static void WriteDirectivesSection(HelpEventInfo eventInfo)
    {
        var symbols = eventInfo.DirectiveSymbols;
        if (symbols.Count == 0)
            return;
        
        var writer = eventInfo.OutputWriter;
        var provider = eventInfo.HelpProvider;
        var lineBounds = new LineBounds(IndentSpaces, eventInfo.DisplayWidth);
        
        writer.WriteLine("Directives:", DisplayElement.Heading);
        var elements = symbols.Select(symbol => DirectiveSymbolElement.Create(provider, symbol));
        writer.WriteTable(elements, lineBounds);
        writer.WriteLine();
    }

    private static void WriteSubCommandsSection(HelpEventInfo eventInfo)
    {
        var (writer, command) = (eventInfo.OutputWriter, eventInfo.Command);
        var provider = eventInfo.HelpProvider;
        var lineBounds = LineBounds.RightJustified(IndentSpaces, eventInfo.DisplayWidth);
        
        if (command.SubCommands.Count == 0) return;
        
        writer.WriteLine("Commands:", DisplayElement.Heading);

        var elements = command
            .SubCommands
            .Select(subCommand => CommandSymbolElement.Create(provider, subCommand));
        
        writer.WriteTable(elements, lineBounds);
        writer.WriteLine();
    }

    private static void WriteUsageSection(HelpEventInfo eventInfo)
    {
        var (writer, command) = (eventInfo.OutputWriter, eventInfo.Command);
        
        writer.WriteLine("Usage:", DisplayElement.Heading);
        WriteUsageSubCommandSyntax(writer, command);
        WriteUsageInvocationSyntax(eventInfo, writer, command);
        WriteUsageHelpSyntax(writer, eventInfo);
        writer.WriteLine();
    }

    private static void WriteUsageInvocationSyntax(HelpEventInfo eventInfo, OutputWriter writer, Command command)
    {
        if (!command.CanCreateCallSite) return;
        
        writer.WriteWhiteSpace(IndentSpaces);
        writer.Write(command.Path, DisplayElement.CommandName);

        var provider = eventInfo.HelpProvider;
        var (arguments, options) = (eventInfo.PositionalSymbols, eventInfo.NamedSymbols);

        switch (arguments.Count)
        {
            case 1:
                writer.WriteWhiteSpace();
                var argumentElement = ArgumentSymbolElement.Create(provider, arguments[0]);
                argumentElement.RenderParameterSyntax(writer);
                break;
            
            case > 1:
                writer.WriteWhiteSpace();
                writer.Write("[arguments]", DisplayElement.ParameterSyntax);
                break;
        }

        if (options.Count > 0)
        {
            writer.WriteWhiteSpace();
            writer.Write("[options]", DisplayElement.ParameterSyntax);
        }

        writer.WriteLine();
    }

    private static void WriteUsageHelpSyntax(OutputWriter writer, HelpEventInfo eventInfo)
    {
        writer.WriteWhiteSpace(IndentSpaces);
        writer.Write(eventInfo.Command.Path, DisplayElement.CommandName);
        writer.WriteWhiteSpace();
        writer.WriteLine(string.Join(" | ", eventInfo.Help.Aliases), DisplayElement.ParameterSyntax);
    }

    private static void WriteUsageSubCommandSyntax(OutputWriter writer, Command command)
    {
        if (command.SubCommands.Count == 0) return;
        writer.WriteWhiteSpace(IndentSpaces);
        writer.Write(command.Path, DisplayElement.CommandName);
        writer.WriteWhiteSpace();
        writer.WriteLine("<command>", DisplayElement.ParameterSyntax);
    }

    private static void WriteCommandDescription(HelpEventInfo eventInfo)
    {
        var provider = eventInfo.HelpProvider;
        var command = eventInfo.Command;
        var remarks = provider.GetRemarks(command);
        var extendedRemarks = provider.GetExtendedRemarks(command).ToArray();

        if (remarks is null && extendedRemarks.Length == 0)
            return;
        
        var writer = eventInfo.OutputWriter;
        writer.WriteLine("Description:", DisplayElement.Heading);

        var lineBounds = LineBounds.RightJustified(IndentSpaces, eventInfo.DisplayWidth);

        if (remarks is not null)
        {
            writer.WriteParagraph(remarks, lineBounds, DisplayElement.Remarks);
            writer.WriteLine();
        }

        foreach (var extendedRemark in extendedRemarks)
        {
            writer.WriteLine(extendedRemark.Title, DisplayElement.Heading);
            writer.WriteParagraph(extendedRemark.Remarks, lineBounds, DisplayElement.Remarks);
            writer.WriteLine();
        }
    }
}