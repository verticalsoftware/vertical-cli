﻿using System.Text;

namespace Vertical.Cli.Help;

internal static class Helpers
{
    delegate void Callback(int index, ReadOnlySpan<char> span);
    
    internal static void AppendWrapped(
        StringBuilder sb,
        string str,
        int width,
        string tab,
        bool appendNewLine)
    {
        BreakString(str, width, (index, span) =>
        {
            if (index > 0)
            {
                sb.AppendLine();
                sb.Append(tab);
            }
            
            sb.Append(span);
        });

        if (appendNewLine)
        {
            sb.AppendLine();
        }
    }
    
    private static void BreakString(string str, int width, Callback callback)
    {
        var span = str.AsSpan();
        var iteration = 0;

        while (span.Length > 0)
        {
            span = TrimLeadingSpace(span);
            span = BreakSpan(span, 
                iteration++, 
                Math.Min(span.Length, width), 
                callback);
        }
    }
 
    private static ReadOnlySpan<char> BreakSpan(
        ReadOnlySpan<char> span,
        int iteration,
        int length, 
        Callback callback)
    {
        if (span.Length == 0)
            return span;
        
        var i = 0;
        for (; i < length; i++)
        {
            switch (span[i])
            {
                case '\r':
                    continue;
                
                case '\n':
                    callback(iteration, span[..i]);
                    return ++i < length ? span[i..] : ReadOnlySpan<char>.Empty;
            }
        }

        if (span.Length <= length)
        {
            callback(iteration, span);
            return ReadOnlySpan<char>.Empty;
        }

        var mark = i;
        
        for (; i > 0 && !char.IsWhiteSpace(span[i-1]); i--)
        {
        }

        if (i == 0) i = mark; // can't break

        callback(iteration, span[..i].ToString());
        return i < span.Length ? span[i..] : ReadOnlySpan<char>.Empty;
    }

    private static ReadOnlySpan<char> TrimLeadingSpace(ReadOnlySpan<char> span)
    {
        var i = 0;
        for (; i < span.Length && span[i] != '\n' && char.IsWhiteSpace(span[i]); i++)
        {
        }

        return i < span.Length ? span[i..] : ReadOnlySpan<char>.Empty;
    }
}