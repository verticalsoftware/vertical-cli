using Vertical.Cli.Configuration;
using Vertical.Cli.Parsing;

namespace Vertical.Cli.Diagnostics;

/// <summary>
/// Indicates an arity error for a directive.
/// </summary>
public sealed class DirectiveArityError : CommandLineError
{
    internal DirectiveArityError(DirectiveSymbol symbol, ArgumentToken token) : base(FormatMessage(symbol))
    {
        Symbol = symbol;
        Token = token;
    }

    /// <summary>
    /// Gets the directive symbol.
    /// </summary>
    public DirectiveSymbol Symbol { get; }

    /// <summary>
    /// Gets the directive token.
    /// </summary>
    public ArgumentToken Token { get; }

    private static string FormatMessage(DirectiveSymbol symbol)
    {
        var identifier = GetSymbolIdentifier(symbol);

        return symbol.Arity == DirectiveParameterArity.NotSupported
            ? $"{identifier}: parameters not supported."
            : $"{identifier}: parameter required.";
    }
}