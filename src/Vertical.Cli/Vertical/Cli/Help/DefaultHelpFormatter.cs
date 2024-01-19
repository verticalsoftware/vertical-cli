using Vertical.Cli.Configuration;

namespace Vertical.Cli.Help;

/// <summary>
/// A built-in renderer for help content.
/// </summary>
public class DefaultHelpFormatter : IHelpFormatter
{
    private static readonly (int Left, int Right) DefaultMargin = (2, 0);
    private const int TermSpacingCount = 5;
    private readonly HelpTextWriter _helpWriter;
    private readonly IHelpProvider _provider;
    
    internal DefaultHelpFormatter(
        IHelpProvider provider, 
        TextWriter textWriter,
        int outputWidth)
    {
        _provider = provider;
        _helpWriter = new HelpTextWriter(textWriter, outputWidth);
    }

    /// <inheritdoc />
    public void WriteContent(ICommandDefinition command)
    {
        var symbols = command
            .GetAllSymbols()
            .Where(symbol => _provider.SymbolSelector(symbol));

        var commands = command
            .GetChildDefinitions()
            .Where(child => _provider.CommandSelector(child));

        var helpFormatInfo = new HelpFormatInfo(command, commands, symbols);
        
        WriteDescription(command);
        WriteUsageGrammar(command, helpFormatInfo);
        WriteCommandList(helpFormatInfo.SubCommands);
        WriteArgumentList(helpFormatInfo.SymbolLookup[SymbolKind.Argument]);
        WriteOptionList(helpFormatInfo.Symbols.Where(symbol => symbol.Kind is SymbolKind.Option or SymbolKind.Switch));
        
        _helpWriter.Flush();
    }
    
    private void WriteDescription(ICommandDefinition command)
    {
        var description = _provider.GetCommandDescription(command);

        if (string.IsNullOrWhiteSpace(description))
            return;
        
        _helpWriter.WriteLine("Description:");
        _helpWriter.Write(description, DefaultMargin);
        _helpWriter.WriteLine(2);
    }

    private void WriteUsageGrammar(ICommandDefinition subject, HelpFormatInfo formatInfo)
    {
        var commandGrammar = _provider.GetCommandUsageGrammar(subject, formatInfo.SubCommands);
        var argumentGrammar = _provider.GetArgumentsUsageGrammar(subject, formatInfo.SymbolLookup[SymbolKind.Argument]);
        var optionGrammar = _provider.GetOptionsUsageGrammar(subject, formatInfo.Symbols.Where(
            symbol => symbol.Kind is SymbolKind.Option or SymbolKind.Switch));
        
        _helpWriter.WriteLine("Usage:");
        _helpWriter.Indent(DefaultMargin.Left);

        foreach (var command in subject.GetContainingCommands())
        {
            WriteWord(_provider.GetCommandName(command), trailingSpace: true);
        }
        
        WriteWord(_provider.GetCommandName(subject), trailingSpace: true);
        WriteWord(commandGrammar, trailingSpace: true);
        WriteWord(argumentGrammar, trailingSpace: true);
        WriteWord(optionGrammar, trailingSpace: true);
        
        _helpWriter.WriteLine(2);
    }

    private void WriteCommandList(IEnumerable<ICommandDefinition> commands)
    {
        if (!commands.Any())
            return;
        
        _helpWriter.WriteLine("Commands:");

        var entries = commands
            .Select(command => (command, name: _provider.GetCommandName(command)))
            .ToArray();

        var justifiedMargin = (entries.Max(entry => entry.name.Length) + TermSpacingCount, 0);

        foreach (var entry in entries)
        {
            _helpWriter.Write(entry.name, DefaultMargin);

            var description = _provider.GetCommandDescription(entry.command);
            if (!string.IsNullOrWhiteSpace(description))
            {
                _helpWriter.Write(description, justifiedMargin);   
            }

            _helpWriter.WriteLine();
        }

        _helpWriter.WriteLine();
    }

    private void WriteArgumentList(IEnumerable<SymbolDefinition> symbols)
    {
        WriteSymbolSection(symbols, "Arguments:");
    }

    private void WriteOptionList(IEnumerable<SymbolDefinition> symbols)
    {
        WriteSymbolSection(symbols, "Options:");
    }

    private void WriteSymbolSection(
        IEnumerable<SymbolDefinition> symbols,
        string sectionTitle)
    {
        if (!symbols.Any())
            return;
        
        _helpWriter.WriteLine(sectionTitle);

        var entries = symbols
            .Select(symbol => (
                key: _provider.GetSymbolSortKey(symbol),
                grammar: _provider.GetSymbolGrammar(symbol),
                description: _provider.GetSymbolDescription(symbol)))
            .OrderBy(entry => entry.key)
            .Select(entry => (entry.grammar, entry.description))
            .ToArray();

        var justifiedMargin = (entries.Max(item => item.grammar.Length) + TermSpacingCount, 0);

        foreach (var (grammar, description) in entries)
        {
            _helpWriter.Write(grammar, DefaultMargin);

            if (!string.IsNullOrWhiteSpace(description))
            {
                _helpWriter.Write(description, justifiedMargin);
            }

            _helpWriter.WriteLine();
        }
        
        _helpWriter.WriteLine();
    }

    private void WriteWord(string? value,
        (int Left, int Right)? margin = null,
        bool leadingSpace = false,
        bool trailingSpace = false)
    {
        if (value == null)
            return;
        
        if (leadingSpace) _helpWriter.WriteSpace();
        
        if (margin.HasValue)
        {
            _helpWriter.Write(value, margin.Value);
        }
        else _helpWriter.Write(value);

        if (trailingSpace) _helpWriter.WriteSpace();
    }
}