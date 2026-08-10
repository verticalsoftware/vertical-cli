using System.Collections;
using Vertical.Cli.Parsing;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents a list of GNU style aliases.
/// </summary>
public sealed class AliasList : IEnumerable<string>
{
    private readonly List<string> _values = [];
    
    internal static readonly AliasList Empty = []; 

    /// <summary>
    /// Adds an value to the list.
    /// </summary>
    /// <param name="value">The alias to add.</param>
    public void Add(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        if (ArgumentSyntax.GetSyntaxKind(value) != SyntaxKind.Option)
        {
            throw new ArgumentException($"Invalid GNU alias '{value}'.", nameof(value));
        }

        _values.Add(value);
    }

    /// <summary>
    /// Gets the alias list values, or generates a result with a single value
    /// created using a default convention.
    /// </summary>
    /// <param name="templateName">The template name.</param>
    /// <returns>String array.</returns>
    public string[] GetValuesOrDefault(string templateName)
    {
        return _values.Count > 0
            ? _values.ToArray()
            : [ArgumentSyntax.CreateGnuAlias(templateName)];
    }
    
    /// <inheritdoc />
    public IEnumerator<string> GetEnumerator() => _values.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}