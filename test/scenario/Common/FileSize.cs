using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Vertical.Cli.ScenarioTests.Common;

public readonly partial struct FileSize : IParsable<FileSize>
{
    public FileSize(int value, string units)
    {
        Value = value;
        Units = units;
    }

    public int Value { get; }

    public string Units { get; }

    /// <inheritdoc />
    public static FileSize Parse(string s, IFormatProvider? provider)
    {
        return TryParse(s, provider, out var result)
            ? result
            : throw new ArgumentException("invalid file size.");
    }

    /// <inheritdoc />
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out FileSize result)
    {
        if (s is not null && MyRegex().Match(s) is { Success: true } match)
        {
            result = new FileSize(int.Parse(match.Groups["count"].Value), match.Groups["unit"].Value);
            return true;
        }

        result = default;
        return false;
    }

    [GeneratedRegex(@"(?<count>\d+)(?<unit>[bkmgBKMG])")]
    private static partial Regex MyRegex();
}