using Vertical.Cli.Diagnostics;

namespace Vertical.Cli.Configuration.Assertion.Types;

/// <summary>
/// Indicates a command aggregate model has a variadic argument that is not in the last
/// ordinal position.
/// </summary>
public sealed class VariadicArgumentPositionAssertion : ConfigurationAssertion
{
    /// <inheritdoc />
    internal VariadicArgumentPositionAssertion(Command command, ICliSymbol[] symbols)
    {
        Command = command;
        Symbols = symbols;
    }

    /// <summary>
    /// Gets the command that has an aggregate model with a variadic argument that is not in the last
    /// ordinal position.
    /// </summary>
    public Command Command { get; }

    /// <summary>
    /// Gets the position arguments of the command.
    /// </summary>
    public ICliSymbol[] Symbols { get; }

    /// <inheritdoc />
    public override string GroupingKey => KeyHelpers.Create(Command);

    /// <inheritdoc />
    public override string GetIssueDescription()
    {
        return "Invalid ordinal position of variadic argument (must be last).";
    }

    /// <inheritdoc />
    public override IEnumerable<string> GetIssueDetail()
    {
        return Symbols.Select(
            symbol => 
                $"{AssertionDescriptor.Create(symbol)} ({(symbol.Arity.IsVariadic ? "variadic" : "fixed")})");
    }
}