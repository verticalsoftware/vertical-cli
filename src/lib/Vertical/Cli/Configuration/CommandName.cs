using Vertical.Cli.Parsing;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents a command name.
/// </summary>
public sealed class CommandName
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandName"/> class.
    /// </summary>
    /// <param name="value">The command name.</param>
    public CommandName(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        if (ArgumentSyntax.GetSyntaxKind(value) != SyntaxKind.None)
        {
            throw new ArgumentException($"Invalid command name '{value}' (cannot match other symbol patterns).");
        }
        
        Value = value;
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    public string Value { get; }
    
    /// <summary>
    /// Implicitly converts a string to a command name.
    /// </summary>
    /// <param name="value">The command name value.</param>
    /// <returns><see cref="CommandName"/></returns>
    public static implicit operator CommandName(string value) => new(value);
}