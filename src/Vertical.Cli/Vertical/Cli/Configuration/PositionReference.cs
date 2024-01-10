namespace Vertical.Cli.Configuration;

/// <summary>
/// Tracks the insert position of symbols.
/// </summary>
internal sealed class PositionReference
{
    internal static PositionReference None { get; } = new() { _next = -1 };
    
    private int _next;

    /// <summary>
    /// Returns the next index.
    /// </summary>
    /// <returns></returns>
    public int Next() => _next++;

    /// <inheritdoc />
    public override string ToString() => _next.ToString();
}