using Vertical.Cli.Binding;
using Vertical.Cli.Configuration;
using Vertical.Cli.Internal;
using Vertical.Cli.Invocation;

namespace Vertical.Cli.Parsing;

public partial class ParseResult
{
    internal static ParseResult Create(
        IParser parser,
        LinkedList<Token> tokenList,
        IReadOnlyList<ISymbol> bindings,
        List<UsageError> errors)
    {
        var symbolBindings = bindings
            .OfType<ISymbolBinding>()
            .ToArray();

        var parseState = new ParseState(parser, tokenList, symbolBindings, errors);

        ParseOptionSymbols(parseState);
        ParseArgumentSymbols(parseState);

        var valuesDictionary = parseState.BuildValuesDictionary();

        SetArityErrors(valuesDictionary, symbolBindings, errors);
        SetInvalidTokenErrors(parser, tokenList, errors);

        return new ParseResult(
            valuesDictionary,
            tokenList.ToArray(),
            symbolBindings,
            errors);
    }

    private static void SetInvalidTokenErrors(IParser parser, LinkedList<Token> tokenList, List<UsageError> errors)
    {
        if (parser.Options.IgnorePendingTokens)
            return;

        errors.AddRange(tokenList
            .Where(token => token.Kind == TokenKind.ArgumentValue)
            .Select(token => new ExtraneousArgumentError(token)));
    }

    private static void SetArityErrors(
        Dictionary<string, string[]> valuesDictionary,
        ISymbolBinding[] symbolBindings,
        List<UsageError> errors)
    {
        errors.AddRange(symbolBindings
            .Where(binding => !binding.HasBindingOptions)
            .Select(binding => (binding, count: valuesDictionary[binding.BindingName].Length))
            .Where(item => !item.binding.Arity.IsCountValid(item.count))
            .Select(item => new ArityError(item.binding, item.count)));
    }

    private static void ParseArgumentSymbols(ParseState state)
    {
        var argumentSymbols = state
            .SymbolBindings
            .Where(binding => binding.Behavior == SymbolBehavior.Argument)
            .OrderBy(binding => binding.Precedence)
            .ToArray();

        var argumentNodes = new Queue<LinkedListNode<Token>>(state
            .TokenList
            .SelectNodes()
            .Where(node => node.Value.Kind is TokenKind.ArgumentValue or TokenKind.TerminatedValue));

        foreach (var symbol in argumentSymbols.TakeWhile(_ => argumentNodes.Count > 0))
        {
            var (count, max) = (0, symbol.Arity.MaxCount ?? int.MaxValue);

            while (count++ < max && argumentNodes.TryDequeue(out var node))
            {
                state.AddArgumentValue(symbol, node.Value);
                state.TokenList.Remove(node);
            }
        }
    }

    private static void ParseOptionSymbols(ParseState state)
    {
        var optionSymbols = BuildOptionSymbols(state.SymbolBindings);

        for (var node = state.TokenList.First; node?.Value is { } token;)
        {
            var consumeNode = TryParseOptionToken(state, optionSymbols, node, token);
            
            node = consumeNode
                ? state.TokenList.Dequeue(node)
                : node.Next;
        }
    }

    private static bool TryParseOptionToken(
        ParseState state,
        Dictionary<string, ISymbolBinding> optionSymbols,
        LinkedListNode<Token> node,
        Token token)
    {
        return token.Kind == TokenKind.OptionSymbol && ParseSemanticOptionToken(state, optionSymbols, node, token);
    }

    private static bool ParseSemanticOptionToken(
        ParseState state,
        Dictionary<string, ISymbolBinding> optionSymbols,
        LinkedListNode<Token> node,
        Token token)
    {
        return state
            .Parser
            .CreateSemanticTokens(token)
            .All(semanticToken => ParseOptionToken(state, optionSymbols, node, semanticToken));
    }

    private static bool ParseOptionToken(
        ParseState state,
        Dictionary<string, ISymbolBinding> optionSymbols,
        LinkedListNode<Token> node,
        SemanticToken semanticToken)
    {
        var symbol = semanticToken.Token.Symbol;

        if (!optionSymbols.TryGetValue(symbol, out var option))
        {
            state.Errors.Add(new InvalidOptionSymbolError(semanticToken.ConstituentToken));
            return false;
        }

        return option.Behavior == SymbolBehavior.Switch
            ? ParseSwitchToken(state, option, semanticToken)
            : ParseParameterizedOptionToken(state, option, node, semanticToken);
    }

    private static bool ParseSwitchToken(
        ParseState state, 
        ISymbolBinding symbolBinding, 
        SemanticToken semanticToken)
    {
        switch (semanticToken)
        {
            case { HasBooleanParameter: true }:
                state.AddParameterValue(symbolBinding, semanticToken.Token);
                return true;
            
            case { Token.Syntax.HasParameter: true }:
                state.Errors.Add(new InvalidSwitchParameterError(symbolBinding, semanticToken.ConstituentToken));
                return false;
            
            default:
                state.AddStringValue(symbolBinding, bool.TrueString);
                return true;
        }
    }

    private static bool ParseParameterizedOptionToken(
        ParseState state,
        ISymbolBinding symbolBinding,
        LinkedListNode<Token> node,
        SemanticToken semanticToken)
    {
        if (semanticToken.Token.Syntax.HasParameter)
        {
            state.AddParameterValue(symbolBinding, semanticToken.Token);
            return true;
        }

        if (node.Next?.Value is { Kind: TokenKind.ArgumentValue })
        {
            state.AddArgumentValue(symbolBinding, node.Next.Value);
            return true;
        }
        
        if (state.TerminatedTokenQueue.TryDequeue(out var terminatedTokenNode))
        {
            state.AddArgumentValue(symbolBinding, terminatedTokenNode.Value);
            state.TokenList.Remove(terminatedTokenNode);
            return true;
        }

        state.Errors.Add(new MissingParameterError(symbolBinding));
        return false;
    }

    private static Dictionary<string, ISymbolBinding> BuildOptionSymbols(ISymbolBinding[] bindings)
    {
        var dictionary = new Dictionary<string, ISymbolBinding>(bindings.Length * 2);
        var aliasProjection = bindings
            .Where(binding => binding.Behavior is SymbolBehavior.Option or SymbolBehavior.Switch)
            .SelectMany(symbol => symbol.Aliases.Select(alias => (alias, synbol: symbol)));

        foreach (var entry in aliasProjection)
        {
            if (dictionary.TryAdd(entry.alias, entry.synbol))
                continue;

            var otherSymbol = dictionary[entry.alias];
            throw Exceptions.ConflictingAlias(entry.synbol, otherSymbol, entry.alias);
        }

        return dictionary;
    }
}