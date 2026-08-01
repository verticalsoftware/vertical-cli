using Vertical.Cli.Configuration;

namespace Vertical.Cli.Diagnostics;

/// <summary>
/// Indicates the minimum input was not met or the maximum input was exceeded for a symbol.
/// </summary>
public sealed class SymbolArityError : CommandLineError
{
    /// <inheritdoc />
    internal SymbolArityError(ICliSymbol symbol, string[] argumentsReceived) 
        : base(FormatMessage(symbol, argumentsReceived.Length))
    {
        ArgumentsReceived = argumentsReceived;
    }

    /// <summary>
    /// Gets the received arguments.
    /// </summary>
    public string[] ArgumentsReceived { get; }

    private static string FormatMessage(ICliSymbol symbol, int argumentCount)
    {
        var identifier = GetSymbolIdentifier(symbol);
        var (min, max) = symbol.Arity;
        var parameterType = symbol.Kind == SymbolKind.PositionArgument
            ? "value"
            : "argument";

        return (min, max, argumentCount) switch
        {
            { argumentCount: 0, min: 1 } => $"{identifier}: {parameterType} required.",
            { } when argumentCount < min => $"{identifier}: {min} {parameterType} required.",
            { max: 1 } => $"{identifier}: single {parameterType} expected.",
            _ => $"{identifier}: no more than {max} {parameterType} expected."
        };
    }
}