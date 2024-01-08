namespace Vertical.Cli;

/// <summary>
/// Represents an empty model.
/// </summary>
public class None
{
    /// <summary>
    /// Defines the single instance of the <see cref="None"/> class.
    /// </summary>
    public static None Default { get; } = new();

    private None()
    {
    }
}