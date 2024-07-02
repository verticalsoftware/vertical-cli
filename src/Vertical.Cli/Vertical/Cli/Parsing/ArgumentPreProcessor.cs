using Vertical.Cli.Internal;

namespace Vertical.Cli.Parsing;

internal sealed class ArgumentPreProcessor
{
    private readonly Func<FileInfo, Stream> _streamFactory;
    private readonly Func<string, string?> _clientProcessor;
    private readonly Stack<Context> _stack = new();
    private readonly HashSet<string> _pathsVisited = [];

    private sealed class ArgumentList(Func<string, string?> processor)
    {
        private readonly List<string> _list = new(64);

        internal void Add(string value)
        {
            var transformed = processor(value);
            if (string.IsNullOrWhiteSpace(transformed))
                return;
            _list.Add(transformed);
        }

        internal void AddRange(IEnumerable<string> values)
        {
            foreach (var value in values)
            {
                Add(value);
            }
        }

        internal string[] ToArray() => _list.ToArray();
    }

    private ArgumentPreProcessor(Func<FileInfo, Stream> streamFactory, Func<string, string?> clientProcessor)
    {
        _streamFactory = streamFactory;
        _clientProcessor = clientProcessor;
    }

    internal readonly record struct Context(FileInfo File, int Line);
    private enum Token { None, Argument, ResponseDirective, Comment }

    internal static string[] Process(string[] arguments, 
        Func<FileInfo, Stream>? streamFactory = null,
        Func<string, string?>? clientProcessor = null)
    {
        for (var c = 0; c < arguments.Length; c++)
        {
            if (arguments[c][0] != '@')
                continue;

            var instance = new ArgumentPreProcessor(
                streamFactory ?? (file => file.OpenRead()),
                clientProcessor ?? (str => str));
            
            return instance.ReadResponseFiles(arguments, c);
        }

        return arguments;
    }

    private string[] ReadResponseFiles(string[] arguments, int i)
    {
        var list = new ArgumentList(_clientProcessor);

        if (i > 0)
        {
            list.AddRange(arguments.Take(i));
        }

        ReadResponseFile(new FileInfo(arguments[i][1..]), list);

        i++;

        for (; i < arguments.Length; i++)
        {
            if (arguments[i][0] != '@')
            {
                list.Add(arguments[i]);
                continue;
            }
            
            ReadResponseFile(new FileInfo(arguments[i][1..]), list);
        }

        return list.ToArray();
    }

    private void ReadResponseFile(FileInfo file, ArgumentList list)
    {
        if (!_pathsVisited.Add(file.FullName))
            return;
        
        try
        {
            using var reader = new StreamReader(_streamFactory(file));
            var lineId = 1;

            while (true)
            {
                var line = reader.ReadLine();
                if (line == null)
                    return;

                _stack.Push(new Context(file, lineId));
                ReadTokens(line, list);
                _stack.Pop();

                lineId++;
            }
        }
        catch (IOException)
        {
            throw Exceptions.ResponseFileNotFound(file, _stack);
        }
    }

    private void ReadTokens(string line, ArgumentList list)
    {
        var span = line.AsSpan();

        while (span.Length > 0)
        {
            SkipWhiteSpace(ref span);
            
            switch (ReadToken(ref span, out var value))
            {
                case Token.Argument:
                    list.Add(value);
                    break;
                
                case Token.ResponseDirective:
                    var current = _stack.Peek();
                    var path = Path.Combine(current.File.DirectoryName!, value);
                    ReadResponseFile(new FileInfo(path), list);
                    break;
            }
        }
    }

    private Token ReadToken(ref ReadOnlySpan<char> span, out string value)
    {
        value = string.Empty;
        
        if (span.Length == 0)
            return Token.None;

        switch (span[0])
        {
            case '"':
                span = span[1..];
                if (!TryReadString(ref span, c => c == '"', out value))
                    throw Exceptions.NonTerminatedQuote(_stack);
                span = span[1..];
                return Token.Argument;
            
            case '#':
                span = [];
                return Token.Comment;

            case '@':
                span = span[1..];
                if (!TryReadString(ref span, char.IsWhiteSpace, out value))
                    throw Exceptions.InvalidResponseFileDirective(_stack);
                return Token.ResponseDirective;

            default:
                TryReadString(ref span, char.IsWhiteSpace, out value);
                return Token.Argument;
        }
    }

    private static bool TryReadString(
        ref ReadOnlySpan<char> span, 
        Predicate<char> stop,
        out string value)
    {
        value = string.Empty;

        if (span.Length == 0)
            return false;

        var i = 0;
        for (; i < span.Length && !stop(span[i]); i++)
        {
        }

        if (i == 0)
        {
            return false;
        }

        value = span[..i].ToString();
        span = span[(Math.Min(span.Length, i))..];
        return true;
    }

    private static void SkipWhiteSpace(ref ReadOnlySpan<char> span)
    {
        if (span.Length == 0)
            return;
        
        var i = 0;
        for (; i < span.Length && char.IsWhiteSpace(span[i]); i++)
        {
        }

        span = span[i..];
    }
}
