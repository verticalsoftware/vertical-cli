using System.Text.RegularExpressions;

namespace Vertical.Cli.Routing;

/// <summary>
/// Represents a route path.
/// </summary>
#if NETSTANDARD2_0
public sealed class RoutePath : IEquatable<RoutePath>, IComparable<RoutePath>
#else
public sealed partial class RoutePath : IEquatable<RoutePath>, IComparable<RoutePath>
#endif
{
#if NETSTANDARD2_0
    private static readonly Regex MyRegexInstance = new("^[a-zA-Z0-9][a-zA-Z0-9-]+$");
    private static Regex MyRegex() => MyRegexInstance;
#else
    [GeneratedRegex("^[a-zA-Z0-9][a-zA-Z0-9-]+$")]
    private static partial Regex MyRegex();
#endif
    
    private readonly Lazy<Regex> lazyMatcher;

    internal RoutePath(string pattern)
    {
        lazyMatcher = new Lazy<Regex>(() => new Regex(pattern));
        Pattern = pattern;
    }

    internal static bool TryParse(string str, out RoutePath path)
    {
        path = default!;
        
        var words = str.Split(' ');

        var valid = words.Length > 0 && words.All(word => !string.IsNullOrWhiteSpace(word)
                                                          && MyRegex().IsMatch(word));

        if (!valid)
        {
            return false;
        }

        path = new RoutePath(str);
        return true;
    }

    /// <summary>
    /// Gets the pattern defined by the route.
    /// </summary>
    public string Pattern { get; }
    
    /// <inheritdoc />
    public bool Equals(RoutePath? other) => Pattern.Equals(other?.Pattern, StringComparison.Ordinal);

    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as RoutePath);

    /// <inheritdoc />
    public int CompareTo(RoutePath? other) => StringComparer.Ordinal.Compare(Pattern, other?.Pattern);

    /// <inheritdoc />
    public override string ToString() => Pattern;

    /// <summary>
    /// Provides a match to the provided path.
    /// </summary>
    /// <param name="path">Path instance</param>
    /// <returns><see cref="Match"/></returns>
    public Match Match(string path) => lazyMatcher.Value.Match(path);

    /// <summary>
    /// Implicitly converts a path string to a <see cref="RoutePath"/> object.
    /// </summary>
    /// <param name="path">Path to convert</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">The path is invalid</exception>
    public static implicit operator RoutePath(string path)
    {
        if (TryParse(path, out var instance))
            return instance;

        throw new ArgumentException(
            "Invalid route path (must be words containing letters, digits, or dash separated by spaces)",
            nameof(path));
    }

    /// <inheritdoc />
    public override int GetHashCode() => Pattern.GetHashCode();
}