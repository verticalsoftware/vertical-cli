using System.Text;
using System.Xml;
using Vertical.Cli.Configuration;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Help;

/// <summary>
/// Writes help content to an xml file.
/// </summary>
public static class XmlHelpWriter
{
    /// <summary>
    /// Writes a file compatible with <see cref="XmlHelpProvider"/> using content defined in code.
    /// </summary>
    /// <param name="stream">The stream to write the help file to.</param>
    /// <param name="application">The application.</param>
    public static void Write(Stream stream, CommandLineApplication application)
    {
        var xmlWriter = XmlWriter.Create(stream, new XmlWriterSettings
        {
            Indent = true,
            IndentChars = "   ",
            Encoding = Encoding.UTF8,
            OmitXmlDeclaration = false,
            NewLineChars = "\n"
        });
        
        WriteRoot(xmlWriter, application);
        xmlWriter.Flush();
    }

    private static void WriteRoot(XmlWriter writer, CommandLineApplication application)
    {
        writer.WriteStartElement("help");

        var commands = GetCommands(application.RootCommand).ToArray();
        
        WriteCommands(writer, commands);
        WriteBoundSymbols(writer, commands, application);
        WriteUnboundSymbols(writer, commands);
        
        writer.WriteEndElement();
    }

    private static void WriteBoundSymbols(XmlWriter writer, Command[] commands, CommandLineApplication app)
    {
        var configuration = app.GetConfiguration();
        var modelTypes = commands
            .Where(command => command.CanCreateCallSite)
            .Select(command => command.ModelType ?? throw new InvalidOperationException())
            .Distinct();

        var symbols = modelTypes
            .Select(configuration.GetModelConfiguration)
            .SelectMany(model => model
                .BindingSources
                .OfType<CliSymbol>())
            .Where(symbol => symbol.HelpTopic is not null)
            .DistinctBy(symbol => (symbol.ModelType, symbol.BindingName));

        foreach (var symbol in symbols)
        {
            WriteSymbolTopic(writer, symbol);
        }
    }

    private static void WriteUnboundSymbols(XmlWriter writer, Command[] commands)
    {
        var unboundSymbols = commands.SelectMany(command => command.DefinedSymbols);

        foreach (var symbol in unboundSymbols)
        {
            WriteSymbolTopic(writer, symbol);
        }
    }

    private static void WriteSymbolTopic(XmlWriter writer, ICliSymbol symbol)
    {
        if (symbol.HelpTopic is not { } helpTopic) return;
        
        writer.WriteStartElement("topic");

        var id = symbol switch
        {
            CliSymbol bound => $"{bound.ModelType.FullName}.{bound.BindingName}",
            IDirectiveSymbol directive => $"(Directive).{directive.Identifier}",
            UnboundSymbol unbound => $"(Unbound).{unbound.Identifier}",
            _ => throw new NotSupportedException($"symbol {symbol} not supported for xml help")
        };
        
        writer.WriteAttributeString("type", "symbol");
        writer.WriteAttributeString("id", id);
        
        if (helpTopic is SymbolHelpTopic { ParameterSyntax: { Length: > 0 } parameterName })
        {
            writer.WriteAttributeString("parameter-name", parameterName);
        }
        
        writer.WriteValue(helpTopic.Remarks);
        writer.WriteEndElement();
    }

    private static void WriteCommands(XmlWriter writer, Command[] commands)
    {
        foreach (var command in commands)
        {
            if (command.HelpTopic is { } helpTopic)
            {
                WriteCommandTopic(writer, command, helpTopic);
            }
        }
    }

    private static void WriteCommandTopic(XmlWriter writer, Command command, CommandHelpTopic helpTopic)
    {
        writer.WriteStartElement("topic");
        writer.WriteAttributeString("type", "command");
        writer.WriteAttributeString("id", command.Path);
        writer.WriteStartElement("remarks");
        writer.WriteValue(helpTopic.Remarks);
        writer.WriteEndElement();

        if (helpTopic.ExtendedRemarks is { Length: > 0 } sections)
        {
            WriteCommandExtendedSections(writer, sections);
        }
        
        writer.WriteEndElement();
    }

    private static void WriteCommandExtendedSections(XmlWriter writer, ExtendedRemarksSection[] sections)
    {
        writer.WriteStartElement("sections");
        
        foreach (var section in sections)
        {
            writer.WriteStartElement("section");
            writer.WriteAttributeString("title", section.Title);
            writer.WriteValue(section.Remarks);
            writer.WriteEndElement();
        }
        
        writer.WriteEndElement();
    }

    private static IEnumerable<Command> GetCommands(RootCommand rootCommand)
    {
        var stack = new Stack<Command>([rootCommand]);
        while (stack.TryPop(out var command))
        {
            yield return command;
            stack.PushRange(command.SubCommands);
        }
    }
}