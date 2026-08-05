namespace Vertical.Cli.Help;

/// <summary>
/// Represents configuration options for the help system.
/// </summary>
public sealed class HelpSystemOptions
{
    /// <summary>
    /// Gets or sets the aliases used to invoke the help system (defaults to <c>--help</c>.
    /// </summary>
    public string[] OptionAliases
    {
        get;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            if (value.Length == 0)
            {
                throw new ArgumentException("Alias list must contain a value");
            }

            // if (value
            //         .Where(alias => ArgumentSyntax.Parse(alias).SyntaxKind is not SyntaxKind.Option)
            //         .ToArray() is { Length: > 0 } invalidAliases)
            // {
            //     throw new ArgumentException(
            //         $"Invalid aliases (must be option syntax): {string.Join(", ", invalidAliases)}");
            // }

            field = value;
        }
    } = ["--help", "-?"];

    /// <summary>
    /// Gets or sets the remarks to display for the help option.
    /// </summary>
    public string OptionRemarks { get; set; } = "Display help for this command.";

    /// <summary>
    /// Gets or sets the article writer instance.
    /// </summary>
    public HelpArticleWriter ArticleWriter { get; set; } = new();

    /// <summary>
    /// Gets or sets the help provider.
    /// </summary>
    public IHelpProvider HelpProvider { get; set; } = new DefaultHelpProvider();
}