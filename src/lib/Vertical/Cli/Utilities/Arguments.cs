using System.Text;

namespace Vertical.Cli.Utilities;

internal static class Arguments
{
    public static IEnumerable<string> ReadAll(TextReader textReader)
    {
        var buffer = new StringBuilder(200);
        var quoting = false;

        while (textReader.ReadLine() is { Length: > 0 } str)
        {
            buffer.Clear();
            
            if (str.Trim().StartsWith('#'))
                continue;
            
            foreach (var chr in str)
            {
                switch (chr)
                {
                    case '"' when quoting:
                        if (buffer.Length > 0)
                        {
                            yield return buffer.ToString();
                        }

                        buffer.Clear();
                        quoting = false;
                        break;
                    
                    case '"':
                        quoting = true;
                        break;
                    
                    case ' ' when !quoting:
                        if (buffer.Length > 0)
                        {
                            yield return buffer.ToString();
                        }

                        buffer.Clear();
                        break;
                        
                    default:
                        buffer.Append(chr);
                        break;
                }
            }

            if (buffer.Length > 0)
            {
                yield return buffer.ToString();
            }
        }
    }

    private static TextReader GetResponseFileTextReader(string arg)
    {
        var path = arg[1..];

        try
        {
            return new StreamReader(File.OpenRead(path));
        }
        catch(Exception exception)
        {
            throw new CliArgumentException(CliArgumentError.InvalidResponseFile,
                $"Could not load response file {path}",
                innerException: exception);
        }
    }
}