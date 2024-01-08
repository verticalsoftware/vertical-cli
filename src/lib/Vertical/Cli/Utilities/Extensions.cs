using Vertical.Cli.Configuration;

namespace Vertical.Cli.Utilities;

internal static class Extensions
{
    internal static string GetPathString(this ICommandDefinition command)
    {
        return string.Join(" -> ", command.EnumerateToRoot().Reverse().Select(cmd => cmd.Id));
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
    
    internal static Type? GetGenericCollectionType(this Type type)
    {
        if (type.IsArray)
        {
            return type.GetElementType();
        }

        if (!type.IsGenericType)
            return null;

        var genericArguments = type.GetGenericArguments();
        
        // Could be Dictionary, KeyValuePair, etc..
        if (genericArguments.Length != 1)
            return null;

        var genericDefinition = type.GetGenericTypeDefinition();
        
        return !SupportedCollectionTypes.Contains(genericDefinition) 
            ? null 
            : genericArguments[0];
    }

    internal static IReadOnlyDictionary<TKey, TValue> ToMergedDictionary<TKey, TValue>(
        this IEnumerable<TValue> values,
        Func<TValue, TKey> keySelector)
        where TKey : notnull
    {
        return values.Aggregate(
            new Dictionary<TKey, TValue>(32),
            (dictionary, next) =>
            {
                dictionary[keySelector(next)] = next;
                return dictionary;
            });
    }

    internal static T GetValueOrDefault<T>(this SymbolDefinition symbol) where T : notnull
    {
        var typedSymbol = (SymbolDefinition<T>)symbol;

        return typedSymbol.DefaultProvider != null
            ? typedSymbol.DefaultProvider()
            : default!;
    }

    private static IEnumerable<ICommandDefinition> EnumerateToRoot(
        this ICommandDefinition? command)
    {
        for (; command != null; command = command.Parent)
        {
            yield return command;
        }
    }
}