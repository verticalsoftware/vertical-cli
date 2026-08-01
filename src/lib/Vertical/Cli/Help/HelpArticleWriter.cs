using Vertical.Cli.Configuration;
using Vertical.Cli.IO;
using Vertical.Cli.Utilities;

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
        WriteCommandAdditionalRemarks(eventInfo);
    }

    private static void WriteCommandAdditionalRemarks(HelpEventInfo eventInfo)
    {
        var (writer, command) = (eventInfo.OutputWriter, eventInfo.Command);
        var provider = eventInfo.HelpProvider;
        var sectionCount = provider.GetCommandSectionsCount(command);

        for (var sectionId = 0; sectionId < sectionCount; sectionId++)
        {
            writer.WriteLine(provider.GetCommandSectionHeading(command, sectionId), DisplayElement.Heading);
            writer.WriteParagraph(
                provider.GetCommandSectionRemarks(command, sectionId),
                new LineBounds(IndentSpaces, eventInfo.DisplayWidth - IndentSpaces),
                DisplayElement.Remarks);
            writer.WriteLine();
        }
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
        if (symbols.Count == 0) return;
        
        var writer = eventInfo.OutputWriter;
        var provider = eventInfo.HelpProvider;
        var lineBounds = new LineBounds(0, eventInfo.DisplayWidth);
        
        writer.WriteLine("Options:", DisplayElement.Heading);

        var elements = symbols.Select(symbol => OptionSymbolElement.Create(provider, symbol));
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
        writer.WriteTable(
            command.SubCommands,
            sub => sub.Name.Length,
            sub => writer.Write(sub.Name, DisplayElement.ListIdentifier),
            provider.GetRemarks,
            DisplayElement.Remarks,
            lineBounds,
            IndentSpaces);
        writer.WriteLine();
    }

    private static void WriteUsageSection(HelpEventInfo eventInfo)
    {
        var (writer, command) = (eventInfo.OutputWriter, eventInfo.Command);
        
        writer.WriteLine("Usage:", DisplayElement.Heading);
        WriteUsageSubCommandSyntax(writer, command);
        WriteUsageInvocationSyntax(eventInfo, writer, command);
        WriteUsageHelpSyntax(writer, command, eventInfo.HelpOptionAliases);
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

    private static void WriteUsageHelpSyntax(OutputWriter writer, Command command, string[] aliases)
    {
        writer.WriteWhiteSpace(IndentSpaces);
        writer.Write(command.Path, DisplayElement.CommandName);
        writer.WriteWhiteSpace();
        writer.WriteLine(string.Join(" | ", aliases), DisplayElement.ParameterSyntax);
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
        if (eventInfo.HelpProvider.GetRemarks(eventInfo.Command) is not { Length: > 0 } remarks)
            return;

        var writer = eventInfo.OutputWriter;
        writer.WriteLine("Description:", DisplayElement.Heading);
        writer.WriteParagraph(remarks, 
            LineBounds.RightJustified(IndentSpaces, eventInfo.DisplayWidth),
            DisplayElement.Remarks);
        writer.WriteLine();
    }
}