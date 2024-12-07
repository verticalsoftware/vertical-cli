namespace Vertical.Cli.Configuration;

/// <summary>
/// Defines an empty model type.
/// </summary>
public sealed class EmptyModel
{
    /// <summary>
    /// Defines a singleton instance of this type.
    /// </summary>
    public static readonly EmptyModel Instance = new();
    
    private EmptyModel()
    {
    }
}