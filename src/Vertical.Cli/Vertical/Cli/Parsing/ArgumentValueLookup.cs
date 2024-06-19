using Vertical.Cli.Configuration;

namespace Vertical.Cli.Parsing;

/// <summary>
/// Utility to create an argument lookup.
/// </summary>
public static class ArgumentValueLookup
{
    internal const string UnmappedArgumentKey = "#unmapped";
    
    private record MappingState(
        Queue<ArgumentSyntax> ArgumentQueue,
        Queue<ArgumentSyntax> DeferredQueue,
        List<(string Key, string Value)> MappedValues,
        Dictionary<string, CliSymbol> OptionSymbols,
        CliSymbol[] ArgumentSymbols);
    
    /// <summary>
    /// Creates an instance of this type.
    /// </summary>
    /// <param name="arguments">Command line argument syntaxes.</param>
    /// <param name="symbols">Aggregated symbols in insertion order.</param>
    /// <returns><see cref="ArgumentValueLookup"/></returns>
    public static ILookup<string, string> Create(
        Queue<ArgumentSyntax> arguments,
        IReadOnlyCollection<CliSymbol> symbols)
    {
        var state = new MappingState(
            arguments,
            new Queue<ArgumentSyntax>(),
            new List<(string Key, string Value)>(arguments.Count * 2),
            symbols
                .Where(symbol => symbol.HasNames)
                .SelectMany(symbol => symbol.Names.Select(name => (name, symbol)))
                .ToDictionary(),
            symbols.Where(symbol => !symbol.HasNames).ToArray());

        MapOptions(state);
        MapArguments(state);

        while (state.DeferredQueue.TryDequeue(out var unmapped))
        {
            state.MappedValues.Add((UnmappedArgumentKey, unmapped.Text));
        }

        return state.MappedValues.ToLookup(pair => pair.Key, pair => pair.Value);
    }

    private static void MapArguments(MappingState state)
    {
        foreach (var symbol in state.ArgumentSymbols)
        {
            MapArgument(state, symbol);
        }
    }

    private static void MapArgument(MappingState state, CliSymbol symbol)
    {
        var count = symbol.Arity.MaxCount.GetValueOrDefault(int.MaxValue);
        while (count-- > 0 && state.DeferredQueue.TryDequeue(out var argument))
        {
            state.MappedValues.Add((symbol.BindingName, argument.Text));
        }
    }

    private static void MapOptions(MappingState state)
    {
        while (state.ArgumentQueue.TryDequeue(out var argument))
        {
            switch (argument.PrefixType)
            {
                case OptionPrefixType.None:
                    state.DeferredQueue.Enqueue(argument);
                    break;
                case OptionPrefixType.PosixOption:
                    MapPosixOptions(state, argument);
                    break;
                case OptionPrefixType.GnuOption:
                default:
                    MapOptionValue(state, argument.IdentifierSymbol, argument.OperandValue);
                    break;
            }
        }
    }

    private static void MapPosixOptions(MappingState state, ArgumentSyntax argument)
    {
        foreach (var identifier in argument.IdentifierName)
        {
            MapOptionValue(state, $"-{identifier}", argument.OperandValue);
        }
    }

    private static void MapOptionValue(MappingState state, string identifier, string value)
    {
        if (!state.OptionSymbols.TryGetValue(identifier, out var symbol))
        {
            state.DeferredQueue.Enqueue(ArgumentSyntax.Parse(identifier));
            return;
        }

        // Resolve the operand
        if (value.Length > 0)
        {
            state.MappedValues.Add((symbol.BindingName, value));
            return;
        }
 
        var hasOperand = state.ArgumentQueue.TryPeek(out var operand) 
                         && operand.PrefixType == OptionPrefixType.None;

        if (symbol.ValueType == typeof(bool) || symbol.ValueType == typeof(bool?))
        {
            if (hasOperand && bool.TryParse(operand!.Text, out _))
            {
                // Not common, but user explicitly specified switch value
                state.ArgumentQueue.Dequeue();
                state.MappedValues.Add((symbol.BindingName, operand.Text));
                return;
            }
            
            // Infer "true"
            state.MappedValues.Add((symbol.BindingName, bool.TrueString));
            return;
        }

        if (!hasOperand)
            return;
        
        // Use this value
        state.MappedValues.Add((symbol.BindingName, operand!.Text));
        state.ArgumentQueue.Dequeue();
    }
}