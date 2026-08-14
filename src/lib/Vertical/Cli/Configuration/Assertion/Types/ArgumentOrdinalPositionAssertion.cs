using Vertical.Cli.Diagnostics;

namespace Vertical.Cli.Configuration.Assertion.Types;

/// <summary>
/// Indicates two or more position arguments share the same ordinal position property.
/// </summary>
public sealed class ArgumentOrdinalPositionAssertion : ConfigurationAssertion
{
    /// <inheritdoc />
    internal ArgumentOrdinalPositionAssertion(Command command, CliSymbol[] symbols)
    {
        Command = command;
        Symbols = symbols;
    }

    /// <summary>
    /// Gets the command.
    /// </summary>
    public Command Command { get; }

    /// <summary>
    /// Gets the argument symbols that share the same ordinal position.
    /// </summary>
    public CliSymbol[] Symbols { get; }

    /// <inheritdoc />
    public override string GroupingKey => KeyHelpers.Create(Command);

    /// <inheritdoc />
    public override string GetIssueDescription()
    {
        return "Position arguments have ambiguous ordinal assignments:";
    }

    /// <inheritdoc />
    public override IEnumerable<string> GetIssueDetail()
    {
        return Symbols.Select(AssertionDescriptor.Create);
    }
}