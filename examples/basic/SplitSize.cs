using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace BasicDemo;

public readonly partial struct SplitSize : IParsable<SplitSize>
{
    public SplitSize(int sizeInBytes)
    {
        SizeInBytes = sizeInBytes;
    }

    public int SizeInBytes { get; }

    /// <inheritdoc />
    public static SplitSize Parse(string s, IFormatProvider? provider)
    {
        return TryParse(s, provider, out var result)
            ? result
            : throw new ArgumentException("invalid file size.");
    }

    /// <inheritdoc />
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out SplitSize result)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(s)) return false;

        if (MyRegex().Match(s) is { Success: true } match)
        {
            var count = int.Parse(match.Groups["count"].Value);
            var multiplier = match.Groups["unit"].Value.ToLower() switch
            {
                "b" => 1,
                "k" => 1000,
                "m" => 1_000_000,
                _ => 1_000_000_000
            };

            result = new SplitSize(count * multiplier);
            return true;
        }

        return false;
    }

    [GeneratedRegex(@"(?<count>\d+)(?<unit>[bkmgBKMG])")]
    private static partial Regex MyRegex();
}