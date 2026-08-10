namespace Vertical.Cli.Help;

/// <summary>
/// Represents a help topic specific to commands.
/// </summary>
public sealed class CommandHelpTopic : HelpTopic
{
    /// <inheritdoc />
    public CommandHelpTopic(string remarks, 
        string[]? invocationSyntaxes = null,
        ExtendedRemarksSection[]? extendedRemarks = null) 
        : base(remarks)
    {
        InvocationSyntaxes = invocationSyntaxes;
        ExtendedRemarks = extendedRemarks;
    }

    /// <summary>
    /// Gets the invocation syntaxes.
    /// </summary>
    public string[]? InvocationSyntaxes { get; }

    /// <summary>
    /// Gets the commands extended remarks.
    /// </summary>
    public ExtendedRemarksSection[]? ExtendedRemarks { get; }

    /// <summary>
    /// Implicitly converts the given remarks to a help topic.
    /// </summary>
    public static implicit operator CommandHelpTopic(string remarks) => new(remarks);
}