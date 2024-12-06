namespace Vertical.Cli.Help;

/// <summary>
/// Creates help providers.
/// </summary>
public static class HelpProviders
{
    /// <summary>
    /// Gets a format provider that display help content in a compact, column-based format.
    /// </summary>
    public static IHelpProvider Compact => CreateCompactFormatProvider();

    /// <summary>
    /// Gets a format provider that displays help content close to the unix "man" style.
    /// </summary>
    public static IHelpProvider UnixStyle => CreateUnixStyleFormatProvider();
    
    /// <summary>
    /// Creates a format provider that writes content in compact form.
    /// </summary>
    /// <param name="formattingOptions">Options that control formatting</param>
    /// <param name="textWriterFactory">Text writer factory</param>
    /// <param name="renderWidth">Render width</param>
    /// <returns><see cref="IHelpProvider"/></returns>
    public static IHelpProvider CreateCompactFormatProvider(
        HelpFormattingOptions? formattingOptions = null,
        Func<TextWriter>? textWriterFactory = null,
        int? renderWidth = null)
    {
        return new CompactFormatProvider(
            formattingOptions ?? new HelpFormattingOptions(), 
            textWriterFactory ?? (() => Console.Out), 
            renderWidth ?? Console.WindowWidth);
    }
    
    /// <summary>
    /// Creates a format provider that writes content in compact form.
    /// </summary>
    /// <param name="formattingOptions">Options that control formatting</param>
    /// <param name="textWriterFactory">Text writer factory</param>
    /// <param name="renderWidth">Render width</param>
    /// <returns><see cref="IHelpProvider"/></returns>
    public static IHelpProvider CreateUnixStyleFormatProvider(
        HelpFormattingOptions? formattingOptions = null,
        Func<TextWriter>? textWriterFactory = null,
        int? renderWidth = null)
    {
        return new UnixStyleFormatProvider(
            formattingOptions ?? UnixStyleFormatProvider.FormattingOptions,
            textWriterFactory ?? (() => Console.Out), 
            renderWidth ?? Console.WindowWidth);
    }
}