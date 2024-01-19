using Vertical.Cli.Utilities;

namespace Vertical.Cli.Configuration;

internal static class SymbolDefinitionExtensions
{
    internal static void ValidateArity(this SymbolDefinition symbol, int argumentCount)
    {
        if (argumentCount < symbol.Arity.MinCount)
        {
            throw InvocationExceptions.MinimumArityNotMet(symbol, argumentCount);
        }

        if (symbol.Arity.MaxCount.HasValue && argumentCount > symbol.Arity.MaxCount)
        {
            throw InvocationExceptions.MaximumArityExceeded(symbol, argumentCount);
        }
    }
    
    internal static string GetShortDisplayString(this SymbolDefinition symbol)
    {
        var identities = symbol.Identities.ToArray();
        var identityCsv = string.Join('|', identities);

        return identities.Length > 1
            ? $"[{identityCsv}]"
            : identityCsv;
    }

    internal static string GetDisplayString(this SymbolDefinition symbol, bool quoteIdentities = false)
    {
        var typeString = symbol.Kind switch
        {
            SymbolKind.Argument => "argument",
            SymbolKind.Option => "option",
            SymbolKind.Switch => "switch",
            _ => symbol.Kind.ToString().ToLower()
        };

        var quotes = quoteIdentities
            ? "\""
            : string.Empty;
        
        return $"{typeString} {quotes}{symbol.GetShortDisplayString()}{quotes}";
    }

    internal static string GetFUllDisplayString(this SymbolDefinition symbol)
    {
        return $"{symbol.Parent.GetPathString()} -> {symbol.GetShortDisplayString()}";
    }
    
    internal static T GetValueOrDefault<T>(this SymbolDefinition symbol)
    {
        var typedSymbol = (SymbolDefinition<T>)symbol;

        return typedSymbol.DefaultProvider != null
            ? typedSymbol.DefaultProvider()
            : default!;
    }
}