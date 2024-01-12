using System.Collections;

namespace Vertical.Cli.Binding;

/// <summary>
/// Represents a lookup of binding values.
/// </summary>
public sealed class BindingLookup : ILookup<string, object>
{
    private readonly ILookup<string, object> _lookup;

    internal BindingLookup(ILookup<string, object> lookup)
    {
        _lookup = lookup;
    }

    /// <inheritdoc />
    public IEnumerator<IGrouping<string, object>> GetEnumerator() => _lookup.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public bool Contains(string key) => _lookup.Contains(key);

    /// <inheritdoc />
    public int Count => _lookup.Count;

    /// <inheritdoc />
    public IEnumerable<object> this[string key] => _lookup[key];
}