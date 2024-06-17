namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents an option symbol.
/// </summary>
/// <typeparam name="TValue">Value type.</typeparam>
public sealed class CliOption<TValue> : CliSymbol<TValue>
{
    /// <inheritdoc />
    internal CliOption(
        CliCommand command,
        string bindingName, 
        string[] names,
        Arity arity,
        CliScope scope,
        Func<TValue>? defaultProvider,
        string? description) 
        : base(command, SymbolType.Option, bindingName, names, arity, scope, defaultProvider, description)
    {
    }
}