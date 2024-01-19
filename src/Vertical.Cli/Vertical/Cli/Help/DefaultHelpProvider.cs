using System.Text;
using Vertical.Cli.Configuration;
using Vertical.Cli.Parsing;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Help;

/// <summary>
/// Renders help content.
/// </summary>
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
        var count = symbols.Count();
        
        if (count == 0)
            return null;

        if (count > 2)
            return "[arguments]";

        var sb = _reusableStringBuilder.GetInstance();
        try
        {
            var pos = 0;
            foreach (var argument in symbols.OrderBy(sym => sym.Position))
            {
                if (pos++ > 0) sb.Append(' ');
                sb.Append(GetSymbolGrammar(argument));
            }

            return sb.ToString();
        }
        finally
        {
            _reusableStringBuilder.Return(sb);
        }
    }

    /// <inheritdoc />
    public string? GetOptionsUsageGrammar(ICommandDefinition command, IEnumerable<SymbolDefinition> symbols)
    {
        return symbols.Any() ? "[options]" : null;
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
            if (symbol.Type == SymbolType.Argument)
            {
                FormatArgumentGrammar(symbol, sb);
            }
            else
            {
                FormatOptionGrammar(symbol, sb);
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
        if (symbol.Type == SymbolType.Argument)
        {
            return $"{symbol.Position:0000}";
        }

        return SortOptionIdentifiers(symbol).First().sortKey;
    }

    /// <inheritdoc />
    public string? GetSymbolArgumentName(SymbolDefinition symbol)
    {
        if (symbol.Type is SymbolType.Switch or SymbolType.HelpOption)
            return null;

        var identities = symbol.Identities;
        var bestId = identities.FirstOrDefault(id => id.Length > 2);
        bestId ??= identities.First();

        return new string(bestId
            .Where(char.IsLetter)
            .Select(char.ToUpper)
            .ToArray());
    }

    private void FormatArgumentGrammar(SymbolDefinition symbol, StringBuilder sb)
    {
        var annotations = symbol.Arity.MinCount > 0
            ? RequiredGrammarTokens
            : OptionalGrammarTokens;

        sb.Append(annotations.First);
        sb.Append(GetSymbolArgumentName(symbol));
        sb.Append(annotations.Last);
    }
    
    private void FormatOptionGrammar(SymbolDefinition symbol, StringBuilder sb)
    {
        var sortedIdentifiers = SortOptionIdentifiers(symbol)
            .Select(item => item.identity);
        
        var pos = 0;
        foreach (var identifier in sortedIdentifiers)
        {
            if (pos++ > 0) sb.Append(", ");
            sb.Append(identifier);
        }

        var argument = GetSymbolArgumentName(symbol);
        if (!string.IsNullOrWhiteSpace(argument))
        {
            sb.Append(' ');
            sb.Append(RequiredGrammarTokens.First);
            sb.Append(argument);
            sb.Append(RequiredGrammarTokens.Last);
        }
        
        if (symbol.Arity.IsMultiValue)
        {
            sb.Append("...");
        }
    }

    private static IEnumerable<(string identity, string sortKey)> SortOptionIdentifiers(SymbolDefinition symbol)
    {
        return symbol
            .Identities
            .Select(identity => (identity, sortKey: SymbolSyntax.Parse(identity).SimpleIdentifiers[0]))
            .OrderBy(item => item.sortKey);
    }
}