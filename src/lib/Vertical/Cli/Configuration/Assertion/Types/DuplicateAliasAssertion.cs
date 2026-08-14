namespace Vertical.Cli.Configuration.Assertion.Types;

/// <summary>
/// Indicates two or more options or switches share an alias.
/// </summary>
public sealed class DuplicateAliasAssertion : ConfigurationAssertion
{
    internal DuplicateAliasAssertion(Command command, string alias, ICliSymbol[] symbols)
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
    public ICliSymbol[] Symbols { get; }

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
        return Symbols.Select(AssertionDescriptor.Create);
    }
}