using Vertical.Cli.Invocation;
using Vertical.Cli.Parsing;

namespace Vertical.Cli.ResponseFiles;

internal sealed class ResponseFileParser(
    IParser parser,
    string resource,
    Func<string, Stream> streamProvider,
    List<UsageError> errors)
{
    private int _linePosition;

    public static Task<LinkedList<Token>> ParseResponseFileTokensAsync(
        InvocationContext context,
        Func<string, Stream> streamProvider)
    {
        return ParseResponseFileTokensAsync(
            context.Parser,
            context.TokenList,
            context.Errors,
            streamProvider);
    }

    public static async Task<LinkedList<Token>> ParseResponseFileTokensAsync(
        IParser parser,
        LinkedList<Token> tokenList,
        List<UsageError> errors,
        Func<string, Stream> streamProvider)
    {
        var insertList = await GetInsertTokenValuesAsync(parser, tokenList, errors, streamProvider);

        if (insertList.Count == 0)
            return tokenList;

        var lookup = insertList.ToLookup(item => item.Node, item => item.InsertValue);
        var valueList = new List<string>(tokenList.Count + insertList.Count);

        for (var node = tokenList.First; node != null;)
        {
            var next = node.Next;
            
            if (!lookup.Contains(node))
            {
                valueList.Add(node.Value.Text);
                node = node.Next;
                continue;
            }
            
            valueList.AddRange(lookup[node]);
            node = next;
        }

        return new LinkedList<Token>(parser.ParseArguments(valueList.ToArray()));
    }

    private static async Task<List<(LinkedListNode<Token> Node, string InsertValue)>> GetInsertTokenValuesAsync(
        IParser tokenParser,
        LinkedList<Token> tokenList,
        List<UsageError> errors,
        Func<string, Stream> streamProvider)
    {
        List<(LinkedListNode<Token>, string)> insertList = [];

        for (var node = tokenList.First; node != null; node = node.Next)
        {
            if (node.Value is not { Kind: TokenKind.Directive, Syntax.EnclosedSpan: ['@', ..] })
                continue;

            var resource = node.Value.Syntax.EnclosedSpan[1..].ToString();
            var parser = new ResponseFileParser(tokenParser, resource, streamProvider, errors);

            await parser.ParseResourceTokensAsync(tokenValue => insertList.Add((node, tokenValue)));
        }

        return insertList;
    }

    public async Task ParseResourceTokensAsync(Action<string> addTokenValue)
    {
        if (CreateTextReader() is not { } textReader)
            return;

        using (textReader)
        {
            await ParseResourceTokensAsync(textReader, addTokenValue);
        }
    }

    private async Task ParseResourceTokensAsync(TextReader textReader, Action<string> addTokenValue)
    {
        while (await textReader.ReadLineAsync() is { } lineValue)
        {
            ++_linePosition;
            ParseLineTokens(addTokenValue, lineValue);
        }
    }

    private void ParseLineTokens(Action<string> addTokenValue, string lineValue)
    {
        if (lineValue.Length == 0)
            return;

        var span = lineValue.AsSpan();
        var c = 0;

        for (; c < span.Length; c++)
        {
            if (span[c] is '\'' or '\"')
            {
                ParseQuotedTokenValue(addTokenValue, span, ref c);
                continue;
            }
                

            if (char.IsWhiteSpace(span[c]))
                continue;

            ParseNonQuotedTokenValue(addTokenValue, span, ref c);
        }
    }

    private void ParseNonQuotedTokenValue(Action<string> addTokenValue, ReadOnlySpan<char> span, ref int c)
    {
        var startPosition = c;
        
        for (; c < span.Length; c++)
        {
            if (!char.IsWhiteSpace(span[c]))
                continue;

            break;
        }

        var value = span[startPosition..c].ToString();

        if (parser.IsTerminatorToken(value))
        {
            errors.Add(new ResponseFileParseError(
                resource,
                _linePosition,
                c,
                "invalid option termination token"));
            return;
        }

        addTokenValue(value);
    }
    
    private void ParseQuotedTokenValue(Action<string> addTokenValue, ReadOnlySpan<char> span, ref int c)
    {
        // c is the opening quote token
        var token = span[c];
        var startPosition = ++c;

        for (; c < span.Length; c++)
        {
            if (span[c] != token)
                continue;

            var innerSpan = span[startPosition..c];
            addTokenValue(innerSpan.ToString());
            ++c;
            return;
        }

        var tokenString = token == '\'' ? "\"'\"" : "'\"'";

        errors.Add(new ResponseFileParseError(
            resource,
            _linePosition,
            c,
            $"expected {tokenString} to close quoted literal"));
    }

    private StreamReader? CreateTextReader()
    {
        try
        {
            return new StreamReader(streamProvider(resource));
        }
        catch (Exception exception)
        {
            errors.Add(new ResponseFileResourceError(resource, exception));
            return null;
        }
    }
}