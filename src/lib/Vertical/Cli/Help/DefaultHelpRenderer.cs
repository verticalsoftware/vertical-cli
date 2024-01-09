using Vertical.Cli.Configuration;
using Vertical.Cli.Parsing;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Help;

/// <summary>
/// A built-in renderer for help content.
/// </summary>
public sealed class DefaultHelpRenderer : IHelpRenderer
{
    private static readonly (int Left, int Right) DefaultMargin = (2, 0);
    private static readonly SymbolSortingComparer SymbolSortingComparer = new();
    private static readonly string[] RequiredAnnotations = { "<", ">" };
    private static readonly string[] OptionalAnnotations = { "[", "]" };
    
    private readonly HelpTextWriter _helpWriter;
    private const int TermSeparatorCount = 3;
    
    internal static DefaultHelpRenderer Instance { get; } = new();

    private DefaultHelpRenderer()
    {
        _helpWriter = new HelpTextWriter(Console.Out, Console.WindowWidth);
    }
    
    /// <inheritdoc />
    public void WriteContent(ICommandDefinition command)
    {
        WriteDescription(command);
        WriteUsage(command);
        WriteCommands(command);
        WriteSymbols("Arguments:", command.GetArgumentSymbols());
        WriteSymbols("Options:", command.GetOptionOrSwitchSymbols());
        _helpWriter.WriteLine();
    }

    private void WriteDescription(ICommandDefinition command)
    {
        if (string.IsNullOrWhiteSpace(command.Description))
            return;
        
        _helpWriter.WriteSection("Description:", command.Description, DefaultMargin);
        _helpWriter.WriteLine(2);
    }

    private void WriteUsage(ICommandDefinition command)
    {
        _helpWriter.WriteLine("Usage:");
        _helpWriter.WriteToMargin(DefaultMargin.Left);
        _helpWriter.Write(command.Id);
        _helpWriter.WriteSpace();

        if (command.HasSubCommands())
        {
            var commandAnnotations = command.HasHandler ? "[]" : "<>";
            _helpWriter.Write($"{commandAnnotations[0]}COMMAND{commandAnnotations[1]}");
            _helpWriter.WriteSpace();
        }

        if (command.GetArgumentSymbols().Any())
        {
            _helpWriter.Write("ARGUMENTS ");   
        }

        if (command.GetOptionOrSwitchSymbols().Any())
        {
            _helpWriter.Write("OPTIONS");
        }

        _helpWriter.WriteLine(2);
    }

    private void WriteCommands(ICommandDefinition command)
    {
        var subCommands = command.CreateChildDefinitions().ToArray();

        if (subCommands.Length == 0)
            return;
        
        _helpWriter.WriteLine("Commands:");

        var subMargin = (subCommands.Max(sub => sub.Id.Length) + TermSeparatorCount + DefaultMargin.Left, 0);

        foreach (var subCommand in subCommands)
        {
            _helpWriter.WriteBlock(subCommand.Id, DefaultMargin);
            
            if (string.IsNullOrWhiteSpace(subCommand.Description))
                continue;
        
            _helpWriter.WriteBlock(subCommand.Description, subMargin);
            _helpWriter.WriteLine();
        }
    }

    private void WriteSymbols(string title, IEnumerable<SymbolDefinition> symbols)
    {
        // Determine sort order
        var sortedSymbols = symbols.OrderBy(s => s, SymbolSortingComparer).ToArray();

        if (sortedSymbols.Length == 0)
            return;

        var leftMargin = sortedSymbols.Max(symbol => symbol.GetShortDisplayString().Length);
        var subMargin = (leftMargin + DefaultMargin.Left + TermSeparatorCount, 0);
        
        _helpWriter.WriteLine(title);

        foreach (var symbol in sortedSymbols)
        {
            var annotations = symbol.Arity.MinCount > 0 ? RequiredAnnotations : OptionalAnnotations;
            
            _helpWriter.WriteBlock(annotations[0], DefaultMargin);

            var identities = symbol
                .Identities
                .OrderBy(id => SymbolSyntax.Parse(id).SimpleIdentifiers![0])
                .ToArray();

            foreach (var (identity, index) in identities.Select((x, c) => (x, c)))
            {
                if (index > 0) _helpWriter.Write(',');
                _helpWriter.Write(identity);
            }
            
            _helpWriter.Write(annotations[1]);
            
            if (!string.IsNullOrWhiteSpace(symbol.Description))
            {
                _helpWriter.WriteBlock(symbol.Description, subMargin);                    
            }
            
            _helpWriter.WriteLine();
        }
        
        _helpWriter.WriteLine();
    }
}