namespace Vertical.Cli.Configuration.Assertion.Types;

/// <summary>
/// Indicates the aggregate model of a command has more than one variadic position argument.
/// </summary>
public sealed class MultipleVariadicArgumentsAssertion : ConfigurationAssertion 
{
    /// <inheritdoc />
    public MultipleVariadicArgumentsAssertion(Command command, ICliSymbol[] symbols)
    {
        Command = command;
        Symbols = symbols;
    }

    /// <summary>
    /// Gets the command that has an aggregated model type with multiple variadic arguments.
    /// </summary>
    public Command Command { get; }

    /// <summary>
    /// Gets the variadic symbols.
    /// </summary>
    public ICliSymbol[] Symbols { get; private set; }

    /// <inheritdoc />
    public override string GroupingKey => KeyHelpers.Create(Command);

    /// <inheritdoc />
    public override string GetIssueDescription()
    {
        return "Multiple variadic arguments in parser configuration";
    }

    /// <inheritdoc />
    public override IEnumerable<string> GetIssueDetail()
    {
        return Symbols.Select(symbol => $"Argument -> {symbol.DisplayName} ({(symbol.Arity.IsVariadic ? "variadic" : "fixed")})");
    }
}