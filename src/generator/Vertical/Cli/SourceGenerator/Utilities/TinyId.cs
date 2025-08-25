namespace Vertical.Cli.SourceGenerator.Utilities;

public static class TinyId
{
    public static string Next => Guid.NewGuid().ToString()[^8..].ToUpper();
}