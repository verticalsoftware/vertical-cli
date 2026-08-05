using Vertical.Cli.Configuration;
using Vertical.Cli.Help;

namespace Vertical.Cli.Diagnostics;

/// <summary>
/// Indicates the client did not provide a parameter value for an option argument.
/// </summary>
public sealed class MissingParameterError : CommandLineError
{
    private MissingParameterError(ICliSymbol symbol, string message) : base(message)
    {
        Symbol = symbol;
    }

    /// <summary>
    /// Gets the symbol.
    /// </summary>
    public ICliSymbol Symbol { get; }

    internal static MissingParameterError Create(ICliSymbol symbol, IHelpProvider helpProvider)
    {
        var identifier = GetSymbolIdentifier(helpProvider, symbol);
        var message =  $"{identifier}: missing required parameter.";

        return new MissingParameterError(symbol, message);
    }
}