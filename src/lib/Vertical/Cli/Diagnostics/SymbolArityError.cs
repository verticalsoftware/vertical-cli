using Vertical.Cli.Configuration;
using Vertical.Cli.Help;

namespace Vertical.Cli.Diagnostics;

/// <summary>
/// Indicates the minimum input was not met or the maximum input was exceeded for a symbol.
/// </summary>
public sealed class SymbolArityError : CommandLineError
{
    private SymbolArityError(ICliSymbol symbol, string[] argumentsReceived, string message) 
        : base(message)
    {
        Symbol = symbol;
        ArgumentsReceived = argumentsReceived;
    }

    /// <summary>
    /// Gets the symbol.
    /// </summary>
    public ICliSymbol Symbol { get; }

    /// <summary>
    /// Gets the received arguments.
    /// </summary>
    public string[] ArgumentsReceived { get; }

    internal static SymbolArityError Create(
        ICliSymbol symbol, 
        string[] argumentsReceived,
        IHelpProvider helpProvider)
    {
        return new SymbolArityError(
            symbol, 
            argumentsReceived, 
            FormatMessage(helpProvider, symbol, argumentsReceived.Length));
    }

    private static string FormatMessage(IHelpProvider helpProvider, ICliSymbol symbol, int argumentCount)
    {
        var identifier = GetSymbolIdentifier(helpProvider, symbol);
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