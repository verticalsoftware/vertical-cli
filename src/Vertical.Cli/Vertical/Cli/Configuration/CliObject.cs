using CommunityToolkit.Diagnostics;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Superclass for symbols and commands.
/// </summary>
public abstract class CliPrimitive
{
    private protected CliPrimitive(
        string[] names,
        string? description,
        string? primaryIdentifier = null)
    {
        Guard.IsNotNull(names);
        
        Names = names;
        PrimaryIdentifier = primaryIdentifier ?? names.FirstOrDefault() ??
            throw new ArgumentException("names[] cannot be empty when primary identifier is null.");
        Description = description;
    }

    /// <summary>
    /// Gets the names known for this object.
    /// </summary>
    public string[] Names { get; }

    /// <summary>
    /// Gets the description.
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// Gets the parent of this item.
    /// </summary>
    public abstract CliPrimitive? Parent { get; }

    /// <summary>
    /// Gets the primary identifier.
    /// </summary>
    public string PrimaryIdentifier { get; }
}