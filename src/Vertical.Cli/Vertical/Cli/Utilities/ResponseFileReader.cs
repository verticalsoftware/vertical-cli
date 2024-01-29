namespace Vertical.Cli.Utilities;

internal static class ResponseFileReader
{
    internal static IEnumerable<string> ReadTokens(TextReader textReader, string source)
    {
        var list = new List<string>(32);

        ReadTokens(textReader, source, list.Add);
        
        return list;
    }

    private static void ReadTokens(TextReader textReader, string source, Action<string> callback)
    {
        var lineNumber = 1;
        
        while (true)
        {
            var lineInput = textReader.ReadLine();

            if (lineInput == null)
                return;

            ReadTokens(lineInput, source, lineNumber++, callback);
        }
    }

    private static void ReadTokens(string input, string source, int lineNumber, Action<string> callback)
    {
        var ptr = 0;
        var len = input.Length;

        // Scan past leading whitespace
        for (; ptr < len && char.IsWhiteSpace(input[ptr]); ptr++)
        {
        }

        var mark = ptr;

        while (ptr < len)
        {
            var chr = input[ptr];

            if (chr is '\'' or '"')
            {
                mark = ++ptr;

                // Scan to end quote
                while (ptr < len && input[ptr] != chr)
                {
                    ++ptr;
                }

                if (ptr > len)
                {
                    // Unterminated quote
                    throw InvocationExceptions.ResponseFileUnterminatedQuote(source, lineNumber, input);
                }

                if (ptr - mark > 1)
                {
                    callback(input[mark..ptr]);
                }
                
                // Scan past end quote
                ptr++;

                mark = ptr;
            }
            else if (char.IsWhiteSpace(chr))
            {
                if (ptr > mark)
                {
                    callback(input[mark..ptr]);
                }

                mark = ++ptr;
            }
            else
            {
                ++ptr;
            }
        }
        
        // Callback remaining
        if (ptr > mark)
        {
            callback(input[mark..]);
        }
    }
}