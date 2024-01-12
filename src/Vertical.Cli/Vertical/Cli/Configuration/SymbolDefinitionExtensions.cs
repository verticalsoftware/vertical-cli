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
        var notations = symbol.Arity.MinCount > 0 ? "<>" : "[]";
        var identities = string.Join('|', symbol.Identities);
        
        return $"{notations[0]}{identities}{notations[1]}";
    }

    internal static string GetDisplayString(this SymbolDefinition symbol)
    {
        var typeString = symbol.Type switch
        {
            SymbolType.Argument => "argument",
            SymbolType.Option => "option",
            SymbolType.Switch => "switch",
            _ => symbol.Type.ToString().ToLower()
        };
        
        return $"{typeString} {symbol.GetShortDisplayString()}";
    }

    internal static string GetFUllDisplayString(this SymbolDefinition symbol)
    {
        return $"{symbol.Parent.GetPathString()} {symbol.GetShortDisplayString()}";
    }
    
    internal static T GetValueOrDefault<T>(this SymbolDefinition symbol)
    {
        var typedSymbol = (SymbolDefinition<T>)symbol;

        return typedSymbol.DefaultProvider != null
            ? typedSymbol.DefaultProvider()
            : default!;
    }
}