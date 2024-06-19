using Vertical.Cli.Binding;

namespace Vertical.Cli.Configuration;

/// <summary>
/// An empty model.
/// </summary>
[NoGeneratorBinding]
public sealed class Empty
{
    private Empty(){}

    /// <summary>
    /// Defines a default instance.
    /// </summary>
    public static readonly Empty Default = new();
}