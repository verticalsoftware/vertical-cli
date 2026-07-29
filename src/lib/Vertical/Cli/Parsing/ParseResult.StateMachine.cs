using System.Diagnostics.CodeAnalysis;
using Vertical.Cli.Configuration;

namespace Vertical.Cli.Parsing;

public partial class ParseResult
{
    private readonly record struct PositionalSymbolState(CliSymbol Symbol, int ArgumentCount);
    
    private sealed class StateMachine(IReadOnlyCollection<CliSymbol> symbols, ITokenList tokenList)
    {
        private readonly Dictionary<string, CliSymbol> _namedSymbols = symbols
            .Where(symbol => symbol.SymbolKind is SymbolKind.Option or SymbolKind.Switch)
            .SelectMany(symbol => symbol.Aliases.Select(alias => (alias, symbol)))
            .ToDictionary(t => t.alias, t => t.symbol);

        private readonly List<PositionalSymbolState> _positionalSymbolStates = symbols
            .Where(symbol => symbol.SymbolKind == SymbolKind.PositionArgument)
            .OrderByDescending(symbol => symbol.OrdinalPosition)
            .Select(symbol => new PositionalSymbolState(symbol, 0))
            .ToList();

        public List<ArgumentToken> UnresolvedTokens { get; } = [];

        public ArgumentToken? CurrentToken { get; private set; } = tokenList.First;

        public List<(CliSymbol Symbol, string ArgumentValue)> ValueMap { get; } = new(32);

        public void MoveNext(int count = 1)
        {
            for (var c = 0; c < count; c++)
            {
                CurrentToken = CurrentToken?.Next;
            }
        }

        public PositionalSymbolState? GetNextPositionalSymbolState()
        {
            return _positionalSymbolStates.Count > 0
                ? _positionalSymbolStates[^1]
                : null;
        }

        public void AddValue(CliSymbol symbol, string argumentValue)
        {
            ValueMap.Add((symbol, argumentValue));
        }

        public void IncrementPositionalSymbolState()
        {
            var state = _positionalSymbolStates[^1];

            if (state.ArgumentCount + 1 == state.Symbol.Arity.Maximum)
            {
                _positionalSymbolStates.RemoveAt(_positionalSymbolStates.Count - 1);
                return;
            }

            _positionalSymbolStates[^1] = state with { ArgumentCount = state.ArgumentCount + 1 };
        }

        public bool TryGetNamedSymbol(ArgumentToken token, [NotNullWhen(true)] out CliSymbol? symbol)
        {
            return (symbol = token.Symbol is { } alias ? _namedSymbols.GetValueOrDefault(alias) : null) != null;
        }
    }
}