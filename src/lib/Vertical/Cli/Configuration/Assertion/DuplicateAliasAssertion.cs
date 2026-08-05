using Vertical.Cli.Configuration.Assertion.Types;

namespace Vertical.Cli.Configuration.Assertion;

/// <summary>
/// Indicates two or more options or switches share an alias.
/// </summary>
public sealed class DuplicateAliasAssertion : ConfigurationAssertion
{
    internal DuplicateAliasAssertion(Command command, string alias, CliSymbol[] symbols)
    {
        Command = command;
        Alias = alias;
        Symbols = symbols;
    }

    /// <summary>
    /// Gets the command that aggregated the options model with the symbols.
    /// </summary>
    public Command Command { get; }
    
    /// <summary>
    /// Gets the shared alias.
    /// </summary>
    public string Alias { get; }

    /// <summary>
    /// Gets the symbols that share the alias.
    /// </summary>
    public CliSymbol[] Symbols { get; }

    /// <inheritdoc />
    public override string GroupingKey => KeyHelpers.Create(Command);

    /// <inheritdoc />
    public override string GetIssueDescription()
    {
        return $"Alias '{Alias}' used across multiple symbols:";
    }

    /// <inheritdoc />
    public override IEnumerable<string> GetIssueDetail()
    {
        return Symbols.Select(symbol => $"{symbol.Kind} {string.Join(", ", symbol.Aliases)}");
    }
}