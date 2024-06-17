namespace Vertical.Cli.SourceGenerator;

public sealed class SeparatorStyle
{
    private SeparatorStyle(
        string token,
        bool itemPerLine)
    {
        Token = token;
        ItemPerLine = itemPerLine;
    }

    public string Token { get; }

    public bool ItemPerLine { get; }

    public static readonly SeparatorStyle CsvList = new(token: ",", itemPerLine: true);
}