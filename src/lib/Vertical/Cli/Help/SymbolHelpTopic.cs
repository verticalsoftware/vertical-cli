namespace Vertical.Cli.Help;

/// <summary>
/// Represents a help topic for symbols.
/// </summary>
public sealed class SymbolHelpTopic : HelpTopic
{
    /// <inheritdoc />
    public SymbolHelpTopic(string remarks, string? parameterSyntax = null) : base(remarks)
    {
        ParameterSyntax = parameterSyntax;
    }

    /// <summary>
    /// Gets the parameter syntax to display.
    /// </summary>
    public string? ParameterSyntax { get; }

    /// <summary>
    /// Implicitly converts a string to this help topic type.
    /// </summary>
    public static implicit operator SymbolHelpTopic(string remarks) => new(remarks);
}