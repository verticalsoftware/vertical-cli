namespace Vertical.Cli.SourceGenerator;

public readonly struct Tab
{
    private const string Spaces = "    ";

    private static readonly string[] PreFormatted =
    {
        "",
        $"{Spaces}",
        $"{Spaces}{Spaces}",
        $"{Spaces}{Spaces}{Spaces}",
        $"{Spaces}{Spaces}{Spaces}{Spaces}",
        $"{Spaces}{Spaces}{Spaces}{Spaces}{Spaces}"
    };

    private readonly int _level;

    public Tab(int indent) => _level = indent;

    public static Tab None => new(0);

    public Tab Indent => new(_level + 1);
    
    public Tab Indent2 => new(_level + 2);
    
    public Tab Indent3 => new(_level + 3);

    /// <inheritdoc />
    public override string ToString() => PreFormatted[_level];
}