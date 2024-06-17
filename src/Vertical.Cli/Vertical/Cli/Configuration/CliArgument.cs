namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents a cli argument.
/// </summary>
/// <typeparam name="TValue">Value type.</typeparam>
public sealed class CliArgument<TValue> : CliSymbol<TValue>
{
    /// <inheritdoc />
    internal CliArgument(
        CliCommand command,
        string bindingName, 
        string[] names,
        Arity arity,
        CliScope scope,
        string? description) 
        : base(command, SymbolType.Argument, bindingName, names, arity, scope, null, description)
    {
    }
}