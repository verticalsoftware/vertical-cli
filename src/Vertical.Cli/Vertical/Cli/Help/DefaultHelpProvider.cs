using System.Text;
using Vertical.Cli.Configuration;
using Vertical.Cli.Parsing;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Help;

public class DefaultHelpProvider : IHelpProvider
{
    private static readonly (string First, string Last) RequiredGrammarTokens = ("<", ">");
    private static readonly (string First, string Last) OptionalGrammarTokens = ("[", "]");

    private readonly Reusable<StringBuilder> _reusableStringBuilder = new(
        () => new StringBuilder(200),
        sb => sb.Clear());
    
    /// <inheritdoc />
    public string? GetCommandDescription(ICommandDefinition command) => command.Description;

    /// <inheritdoc />
    public string GetApplicationName(ICommandDefinition rootCommand) => rootCommand.Id;

    /// <inheritdoc />
    public string GetCommandName(ICommandDefinition command) => command.Id;

    /// <inheritdoc />
    public Predicate<SymbolDefinition> SymbolSelector => _ => true;

    /// <inheritdoc />
    public Predicate<ICommandDefinition> CommandSelector => _ => true;

    /// <inheritdoc />
    public string? GetCommandUsageGrammar(
        ICommandDefinition command,
        IReadOnlyCollection<ICommandDefinition> subCommands)
    {
        if (!subCommands.Any())
            return null;

        var grammarTokens = command.HasHandler ? OptionalGrammarTokens : RequiredGrammarTokens;
        return $"{grammarTokens.First}command{grammarTokens.Last}";
    }

    /// <inheritdoc />
    public string? GetArgumentsUsageGrammar(ICommandDefinition command, IEnumerable<SymbolDefinition> symbols)
    {
        return symbols.Any() ? "arguments" : null;
    }

    /// <inheritdoc />
    public string? GetOptionsUsageGrammar(ICommandDefinition command, IEnumerable<SymbolDefinition> symbols)
    {
        return symbols.Any() ? "options" : null;
    }

    /// <inheritdoc />
    public string? GetSymbolDescription(SymbolDefinition symbol)
    {
        return symbol.Description;
    }

    /// <inheritdoc />
    public string GetSymbolGrammar(SymbolDefinition symbol)
    {
        var sb = _reusableStringBuilder.GetInstance();

        try
        {
            var count = 0;
            foreach (var identity in symbol.Identities)
            {
                if (count++ > 0) sb.Append(',');
                sb.Append(identity);
            }

            var argumentName = GetSymbolArgumentName(symbol);

            if (!string.IsNullOrWhiteSpace(argumentName))
            {
                sb.Append(' ');
                sb.Append('<');
                sb.Append(argumentName);
                sb.Append('>');
            }

            if (symbol.Arity.MaxCount is null or > 0)
            {
                sb.Append("...");
            }

            return sb.ToString();
        }
        finally
        {
            _reusableStringBuilder.Return(sb);
        }
    }

    /// <inheritdoc />
    public string GetSymbolSortKey(SymbolDefinition symbol)
    {
        return SymbolSyntax.Parse(symbol.Id).SimpleIdentifiers![0];
    }

    /// <inheritdoc />
    public string? GetSymbolArgumentName(SymbolDefinition symbol)
    {
        if (symbol.Type == SymbolType.Switch)
            return null;

        return new string(symbol
            .Id
            .Where(char.IsLetter)
            .Select(char.ToUpper)
            .ToArray());
    }
}