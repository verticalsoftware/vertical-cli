using Vertical.Cli.Configuration;
using Vertical.Cli.IO;

namespace Vertical.Cli.Help;

/// <summary>
/// Displays help to the console output.
/// </summary>
public class HelpArticleWriter
{
    private const int IndentSpaces = 2;
    
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

        var items = symbols
            .Select(symbol => new
            {
                symbol,
                identifier = FormatArityEnclosure(provider.GetListIdentifier(symbol), symbol.Arity)
            });

        writer.WriteTable(
            items,
            item => item.identifier.Length + 2,
            item =>
            {
                if (item.symbol.Arity.Minimum > 0)
                    writer.Write("* ", DisplayElement.Important);
                else
                    writer.WriteWhiteSpace(3);
                writer.Write(item.identifier, DisplayElement.ListIdentifier);
            },
            item => provider.GetRemarks(item.symbol),
            DisplayElement.Remarks,
            lineBounds,
            IndentSpaces);

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

        var items = symbols
            .Select(symbol => new
            {
                required = symbol.Arity.Minimum > 0,
                identifier = provider.GetListIdentifier(symbol),
                parameter = FormatArityEnclosure(
                    provider.GetParameterValueSyntax(symbol), 
                    GetOptionParameterArity(symbol)),
                remarks = provider.GetRemarks(symbol)
            });

        writer.WriteTable(
            items,
            item => item.identifier.Length
                    + item.parameter.Length
                    + 3,
            item =>
            {
                if (item.required)
                    writer.Write("* ", DisplayElement.Important);
                else
                    writer.WriteWhiteSpace(2);
                
                writer.Write(item.identifier, DisplayElement.ListIdentifier);
                writer.WriteWhiteSpace();
                
                if (item.parameter.Length > 0)
                {
                    writer.Write(item.parameter, DisplayElement.ParameterSyntax);
                }
            },
            item => item.remarks,
            DisplayElement.Remarks,
            lineBounds,
            IndentSpaces);
        
        writer.WriteLine();
    }
    
    private static void WriteDirectivesSection(HelpEventInfo eventInfo)
    {
        var directives = eventInfo.Directives;
        if (directives.Count == 0) return;
        
        var writer = eventInfo.OutputWriter;
        var provider = eventInfo.HelpProvider;
        var lineBounds = new LineBounds(0, eventInfo.DisplayWidth);
        
        writer.WriteLine("Available directives:", DisplayElement.Heading);

        var tableItems = directives
            .Select(directive => new
            {
                identifier = provider.GetListIdentifier(directive),
                parameter = GetDirectiveParameterSyntax(provider, directive),
                remarks = provider.GetRemarks(directive)
            });

        writer.WriteTable(
            tableItems,
            item => item.identifier.Length + item.parameter.Length + 5,
            item =>
            {
                writer.WriteWhiteSpace(2);
                writer.Write('[', DisplayElement.ListIdentifier);
                writer.Write(item.identifier, DisplayElement.ListIdentifier);
                if (item.parameter.Length > 0)
                {
                    writer.Write('=', DisplayElement.ParameterSyntax);
                    writer.Write(item.parameter, DisplayElement.ParameterSyntax);
                }
                writer.Write(']');
                if (item.parameter.Length == 0) writer.WriteWhiteSpace();
            },
            item => item.remarks,
            DisplayElement.Remarks,                
            lineBounds,
            IndentSpaces);
        
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
                var argumentSyntax = provider.GetListIdentifier(arguments[0]);
                writer.Write(FormatArityEnclosure(argumentSyntax, arguments[0].Arity), DisplayElement.ParameterSyntax);
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

    private static Arity GetOptionParameterArity(CliSymbol symbol) => symbol.Arity.Maximum is null or > 1
        ? Arity.OneOrMore
        : Arity.One;

    private static string FormatArityEnclosure(string? parameterSyntax, Arity arity)
    {
        if (parameterSyntax is not { Length: > 0 }) return string.Empty;
        
        return arity switch
        {
            { Minimum: 0, Maximum: 1 } => $"[{parameterSyntax}]",
            { Minimum: 0, Maximum: null } => $"[{parameterSyntax} ...]",
            { Minimum: 1, Maximum: 1 } => $"<{parameterSyntax}>",
            { Minimum: 1, Maximum: null } => $"<{parameterSyntax} [...]>",
            _ => $"<{parameterSyntax} ...>"
        };
    }

    private static string GetDirectiveParameterSyntax(IHelpProvider helpProvider, DirectiveSymbol directive)
    {
        var parameterSyntax = helpProvider.GetParameterValueSyntax(directive);
        
        return directive.Arity switch
        {
            DirectiveParameterArity.NotSupported => string.Empty,
            DirectiveParameterArity.Optional => FormatArityEnclosure(parameterSyntax, Arity.ZeroOrOne),
            _ => FormatArityEnclosure(parameterSyntax, Arity.One)
        };
    }
}