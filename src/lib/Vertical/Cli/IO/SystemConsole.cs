namespace Vertical.Cli.IO;

/// <summary>
/// Represents the default system console.
/// </summary>
public sealed class SystemConsole : IConsole
{
    /// <inheritdoc />
    public TextReader In => Console.In;

    /// <inheritdoc />
    public TextWriter Out => Console.Out;

    /// <inheritdoc />
    public bool IsOutputRedirected => Console.IsOutputRedirected;

    /// <inheritdoc />
    public int DisplayWidth => Console.WindowWidth;
}