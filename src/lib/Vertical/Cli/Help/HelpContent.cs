using CommunityToolkit.Diagnostics;

namespace Vertical.Cli.Help;

public sealed class HelpContent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HelpContent"/> class.
    /// </summary>
    /// <param name="text">The text to display.</param>
    /// <param name="operandName">Option name to display for the operand.</param>
    public HelpContent(string text,
        string? operandName = null)
    {
        Text = text;
        OperandName = operandName; 
    }

    /// <summary>
    /// Gets the help text.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the operand name.
    /// </summary>
    public string? OperandName { get; }

    public static implicit operator HelpContent(string str)
    {
        Guard.IsNullOrWhiteSpace(str);
        return new HelpContent(str);
    }
}