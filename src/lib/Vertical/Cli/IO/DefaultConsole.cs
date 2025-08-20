using System.Diagnostics.CodeAnalysis;

namespace Vertical.Cli.IO;

/// <summary>
/// Implements <see cref="IConsole"/> using the <see cref="System.Console"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class DefaultConsole : IConsole
{
    /// <summary>
    /// Gets the foreground color.
    /// </summary>
    public ConsoleColor ForegroundColor { get; set; } = Console.ForegroundColor;

    /// <inheritdoc />
    public TextReader In => Console.In;

    /// <inheritdoc />
    public TextWriter Out => Console.Out;

    /// <inheritdoc />
    public int DisplayWidth => Math.Max(120, Console.WindowWidth);

    /// <inheritdoc />
    public bool ErrorMode
    {
        get => Console.ForegroundColor == ConsoleColor.Red;
        set => Console.ForegroundColor = value ? ConsoleColor.Red : ForegroundColor;
    }

    /// <inheritdoc />
    public void HandleCancelEvent(Action handleCancel)
    {
        Console.CancelKeyPress += (_, args) =>
        {
            handleCancel();
            args.Cancel = true;
        };
    }

    /// <inheritdoc />
    public void Write(string str)
    {
        Console.Write(str);
    }

    /// <inheritdoc />
    public void WriteLine()
    {
        Console.WriteLine();
    }

    /// <inheritdoc />
    public void WriteLine(string str)
    {
        Console.WriteLine(str);
    }

    /// <inheritdoc />
    public void WriteErrorLine(string str)
    {
        var errorMode = ErrorMode;

        try
        {
            WriteLine(str);
        }
        finally
        {
            ErrorMode = errorMode;
        }
    }
}