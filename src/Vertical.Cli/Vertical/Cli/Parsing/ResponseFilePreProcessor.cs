using Vertical.Cli.Internal;

namespace Vertical.Cli.Parsing;

/// <summary>
/// Defines a pre-processor that injects arguments read from response files.
/// </summary>
public static class ResponseFilePreProcessor
{
    internal readonly record struct StackFrame(FileInfo File, int Line);

    private record ProcessorContext(LinkedList<string> ArgumentList, Func<FileInfo, Stream> StreamProvider)
    {
        internal Stack<StackFrame> Stack { get; } = new();
        internal HashSet<string> PathsVisited { get; } = [];
    }
    
    private enum Token { None, Argument, ResponseDirective, Comment }

    /// <summary>
    /// Performs pre-processing using the default stream provider.
    /// </summary>
    /// <param name="argumentList">The mutable argument list.</param>
    public static void Handle(LinkedList<string> argumentList)
    {
        Handle(argumentList, fileInfo => File.OpenRead(fileInfo.FullName));
    }
    
    /// <summary>
    /// Performs pre-processing using the default stream provider, and invokes the next
    /// pre-processor.
    /// </summary>
    /// <param name="argumentList">The mutable argument list.</param>
    /// <param name="next">An action that invokes the next pre-processor.</param>
    public static void Handle(LinkedList<string> argumentList, Action<LinkedList<string>> next)
    {
        Handle(argumentList, fileInfo => File.OpenRead(fileInfo.FullName));
        next(argumentList);
    }

    /// <summary>
    /// Performs pre-processing using the specified stream provider.
    /// </summary>
    /// <param name="argumentList">The mutable argument list.</param>
    /// <param name="streamProvider">A function that returns a readable stream.</param>
    public static void Handle(LinkedList<string> argumentList, Func<FileInfo, Stream> streamProvider)
    {
        for (var node = argumentList.First; node != null; node = node.Next)
        {
            TryReadNodeValueAsResponseFilePath(argumentList, streamProvider, ref node);
        }
    }

    private static void TryReadNodeValueAsResponseFilePath(LinkedList<string> argumentList,
        Func<FileInfo, Stream> streamProvider,
        ref LinkedListNode<string> node)
    {
        if (!node.Value.StartsWith('@'))
            return;

        var thisNode = node;

        TryReadAndHandleArguments(
            new FileInfo(node.Value[1..]),
            new ProcessorContext(argumentList, streamProvider),
            ref node);

        argumentList.Remove(thisNode);
    }

    private static void TryReadAndHandleArguments(
        FileInfo fileInfo, 
        ProcessorContext context, 
        ref LinkedListNode<string> insertPosition)
    {
        if (!context.PathsVisited.Add(fileInfo.FullName))
            return;

        try
        {
            using var streamReader = new StreamReader(context.StreamProvider(fileInfo));
            ReadAndHandleArgumentsFromStream(
                fileInfo,
                streamReader,
                context,
                ref insertPosition);
        }
        catch (IOException)
        {
            throw Exceptions.ResponseFileNotFound(fileInfo, context.Stack);
        }
    }

    private static void ReadAndHandleArgumentsFromStream(
        FileInfo fileInfo, 
        StreamReader streamReader, 
        ProcessorContext context, 
        ref LinkedListNode<string> insertPosition)
    {
        var stack = context.Stack;
        var lineId = 1;

        while (streamReader.ReadLine() is { } lineContent)
        {
            if (!string.IsNullOrWhiteSpace(lineContent))
            {
                stack.Push(new StackFrame(fileInfo, lineId));
                ReadTokens(context, lineContent, ref insertPosition);
                stack.Pop();
            }

            ++lineId;
        }
    }

    private static void ReadTokens(
        ProcessorContext context, 
        string lineContent, 
        ref LinkedListNode<string> insertPosition)
    {
        var span = lineContent.AsSpan();
        var list = context.ArgumentList;

        while (span.Length > 0)
        {
            SkipWhiteSpace(ref span);

            switch (ReadToken(context, ref span, out var value))
            {
                case Token.Argument:
                    insertPosition = list.AddAfter(insertPosition, value);
                    break;
                
                case Token.ResponseDirective:
                    TryReadAndHandleArguments(new FileInfo(value), context, ref insertPosition);
                    break;
            }
        }
    }

    private static Token ReadToken(ProcessorContext context, ref ReadOnlySpan<char> span, out string value)
    {
        value = string.Empty;
        
        if (span.Length == 0)
            return Token.None;

        switch (span[0])
        {
            case '"':
                span = span[1..];
                if (!TryReadString(ref span, c => c == '"', out value))
                    throw Exceptions.NonTerminatedQuote(context.Stack);
                span = span[1..];
                return Token.Argument;
            
            case '#':
                span = [];
                return Token.Comment;

            case '@':
                span = span[1..];
                if (!TryReadString(ref span, char.IsWhiteSpace, out value))
                    throw Exceptions.InvalidResponseFileDirective(context.Stack);
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