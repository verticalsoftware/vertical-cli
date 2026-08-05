namespace Vertical.Cli.IO;

/// <summary>
/// Writes control codes to the output that affect how visual elements are displayed.
/// </summary>
public class OutputFormatter
{
    private readonly Dictionary<DisplayElement, string> _theme;

    private static Dictionary<DisplayElement, string> _defaultTheme => new()
    {
        [DisplayElement.Important] = "\e[0;22;31m"
    };

    private static Dictionary<DisplayElement, string> _verticalTheme => new()
    {
        [DisplayElement.Important] = "\e[0;22;31m",
        [DisplayElement.Heading] = "\e[97m",
        [DisplayElement.Remarks] = "\e[38;5;250m",
        [DisplayElement.CommandName] = "\e[38;5;3m",
        [DisplayElement.ParameterSyntax] = "\e[38;5;245m",
        [DisplayElement.ListIdentifier] = "\e[38;5;250m",
        [DisplayElement.RequiredAnnotation] = "\e[38;5;208m"
        //[DisplayElement.RequiredAnnotation] = "\e[38;5;166m"
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="OutputFormatter"/> class.
    /// </summary>
    /// <param name="theme">A dictionary containing formatting codes for the display elements.</param>
    public OutputFormatter(Dictionary<DisplayElement, string>? theme = null)
    {
        _theme = theme ?? _defaultTheme;
    }
    
    /// <summary>
    /// Writes an ANSI formatting control sequence.
    /// </summary>
    /// <param name="textWriter">The text writer where output is being directed to.</param>
    /// <param name="element">The element type being written immediately after the control code.</param>
    public virtual void WriteControlSequence(TextWriter textWriter, DisplayElement element)
    {
        var controlCode = _theme.GetValueOrDefault(element) ?? "\e[0m";
        textWriter.Write(controlCode);
    }

    /// <summary>
    /// Defines a default themed formatter.
    /// </summary>
    public static OutputFormatter Default => new(_defaultTheme);
    
    /// <summary>
    /// Defines the vertical software themed formatter.
    /// </summary>
    public static OutputFormatter VerticalTheme => new(_verticalTheme);
}