using Vertical.Cli.Binding;
using Vertical.Cli.Internal;
using Vertical.Cli.Invocation;

namespace Vertical.Cli.Parsing;

internal sealed class ParseState
{
    public ParseState(IParser parser, 
        LinkedList<Token> tokenList, 
        ISymbolBinding[] symbolBindings,
        List<UsageError> errors)
    {
        Parser = parser;
        TokenList = tokenList;
        SymbolBindings = symbolBindings;
        Errors = errors;
        ValueCollection = new List<(string Key, string Value)>(tokenList.Count);
        TerminatedTokenQueue = new Queue<LinkedListNode<Token>>(tokenList
            .SelectNodes()
            .Where(node => node.Value.Kind == TokenKind.TerminatedValue));
    }


    public List<(string Key, string Value)> ValueCollection { get; }

    public HashSet<ISymbolBinding> ReportedSymbols { get; } = [];

    public IParser Parser { get; }

    public LinkedList<Token> TokenList { get; }
    
    public Queue<LinkedListNode<Token>> TerminatedTokenQueue { get; }

    public ISymbolBinding[] SymbolBindings { get; }

    public List<UsageError> Errors { get; }

    public void AddParameterValue(ISymbolBinding binding, Token token)
    {
        ValueCollection.Add((binding.BindingName, token.Syntax.ParameterSpan.ToString()));
    }

    public void AddArgumentValue(ISymbolBinding binding, Token token)
    {
        ValueCollection.Add((binding.BindingName, token.Text));
    }

    public void AddStringValue(ISymbolBinding binding, string value)
    {
        ValueCollection.Add((binding.BindingName, value));
    }

    public Dictionary<string, string[]> BuildValuesDictionary()
    {
        var keyValuePairs = ValueCollection
            .GroupBy(entry => entry.Key)
            .Select(grouping => new KeyValuePair<string, string[]>(
                grouping.Key,
                grouping.Select(g => g.Value).ToArray()));

        var dictionary = new Dictionary<string, string[]>(keyValuePairs);

        // Add empty arrays for non-matched symbol bindings
        foreach (var binding in SymbolBindings.Where(item => !dictionary.ContainsKey(item.BindingName)))
        {
            dictionary.Add(binding.BindingName, []);
        }

        return dictionary;
    }
}