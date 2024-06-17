namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents a switch symbol.
/// </summary>
public sealed class CliSwitch : CliSymbol<bool>
{
    /// <inheritdoc />
    internal CliSwitch(
        CliCommand command,
        string bindingName,
        string[] names,
        CliScope scope,
        Func<bool>? defaultProvider,
        string? description)
        : base(
            command,
            SymbolType.Switch,
            bindingName, 
            names, 
            Arity.ZeroOrOne, scope, 
            defaultProvider ?? (() => true), 
            description)
    {
    }
}