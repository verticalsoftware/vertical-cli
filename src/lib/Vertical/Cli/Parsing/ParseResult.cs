using System.Diagnostics.CodeAnalysis;
using Vertical.Cli.Configuration;
using Vertical.Cli.Diagnostics;

namespace Vertical.Cli.Parsing;

/// <summary>
/// Represents the result of parsing a set of argument tokens using a list of defined
/// symbols.
/// </summary>
public partial class ParseResult
{
    private readonly ILookup<string, string> _bindingLookup;

    private ParseResult(ILookup<string, string> bindingLookup, List<ArgumentToken> unresolvedTokens)
    {
        UnresolvedTokens = unresolvedTokens;
        _bindingLookup = bindingLookup;
    }

    /// <summary>
    /// Gets the list of tokens that were not resolved.
    /// </summary>
    public List<ArgumentToken> UnresolvedTokens { get; }

    /// <summary>
    /// Gets an enumeration of <see cref="UnresolvedTokenError"/> objects.
    /// </summary>
    public IEnumerable<UnresolvedTokenError> GetUnresolvedTokenErrors() => UnresolvedTokens
        .Select(token => new UnresolvedTokenError(token));

    /// <summary>
    /// Gets all values associated with the given binding name.
    /// </summary>
    /// <param name="bindingName">The binding name the value is associated with the values.</param>
    /// <returns>An enumeration of zero or more matched string values.</returns>
    public IEnumerable<string> GetArgumentValues(string bindingName)
    {
        return _bindingLookup[bindingName];
    }

    internal static ParseResult Create(IReadOnlyCollection<CliSymbol> symbols, ITokenList tokenList)
    {
        var state = new StateMachine(symbols, tokenList);

        while (state.CurrentToken is { } token)
        {
            switch (token)
            {
                case { Kind: TokenKind.Option }:
                    EvaluateOptionToken(state, token);
                    break;
                
                case { Kind: TokenKind.CommandOrArgument or TokenKind.Argument }:
                    EvaluatePositionArgumentToken(state, token);
                    break;
                
                default:
                    state.UnresolvedTokens.Add(token);
                    state.MoveNext();
                    break;
            }
        }

        var bindingLookup = state
            .ValueMap
            .ToLookup(entry => entry.Symbol.BindingName, entry => entry.ArgumentValue);

        return new ParseResult(bindingLookup, state.UnresolvedTokens);
    }

    private static void EvaluatePositionArgumentToken(StateMachine state, ArgumentToken token)
    {
        var positionalSymbolState = state.GetNextPositionalSymbolState();

        try
        {
            if (positionalSymbolState is null)
            {
                state.UnresolvedTokens.Add(token);
                return;
            }
            
            state.AddValue(positionalSymbolState.Value.Symbol, token.Text);
            state.IncrementPositionalSymbolState();
        }
        finally
        {
            state.MoveNext();
        }
    }

    private static void EvaluateOptionToken(StateMachine state, ArgumentToken token)
    {
        if (!state.TryGetNamedSymbol(token, out var symbol))
        {
            state.UnresolvedTokens.Add(token);
            state.MoveNext();
            return;
        }

        switch (token, symbol)
        {
            case { symbol.Kind: SymbolKind.Switch }:
                state.AddValue(symbol, bool.TrueString);
                break;
                
            case { token.Value: { } attachedParameter }:
                state.AddValue(symbol, attachedParameter);
                break;
            
            case { token.Next: { Kind: TokenKind.Argument } nextToken }:
                state.AddValue(symbol, nextToken.Text);
                state.MoveNext();
                break;
            
            default:
                state.AddValue(symbol, string.Empty);
                break;
        }

        state.MoveNext();
    }
}