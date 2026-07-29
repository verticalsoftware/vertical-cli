using Vertical.Cli.Configuration;

namespace Vertical.Cli.Diagnostics;

/// <summary>
/// Indicates the client did not provide a parameter value for an option argument.
/// </summary>
public sealed class MissingParameterError : CommandLineError
{
    internal MissingParameterError(ICliSymbol symbol) : base(FormatMessage(symbol))
    {
        Symbol = symbol;
    }

    public ICliSymbol Symbol { get; }

    private static string FormatMessage(ICliSymbol symbol)
    {
        var identifier = GetSymbolIdentifier(symbol);
        return $"{identifier}: missing required parameter.";
    }
}