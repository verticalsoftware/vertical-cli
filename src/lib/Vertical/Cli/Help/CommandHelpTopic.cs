namespace Vertical.Cli.Help;

/// <summary>
/// Represents a help topic specific to commands.
/// </summary>
public sealed class CommandHelpTopic : HelpTopic
{
    /// <inheritdoc />
    public CommandHelpTopic(string remarks, 
        string[]? invocationSyntaxes = null,
        HelpContentSection[]? sectionContent = null) 
        : base(remarks)
    {
        InvocationSyntaxes = invocationSyntaxes;
        SectionContent = sectionContent;
    }

    /// <summary>
    /// Gets the invocation syntaxes.
    /// </summary>
    public string[]? InvocationSyntaxes { get; }

    /// <summary>
    /// Gets the section content.
    /// </summary>
    public HelpContentSection[]? SectionContent { get; }

    public static implicit operator CommandHelpTopic(string remarks) => new(remarks);
}