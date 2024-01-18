using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Vertical.Cli.Binding;

/// <summary>
/// Base type for argument bindings.
/// </summary>
public abstract class ArgumentBinding : IEnumerable<object>
{
    internal ArgumentBinding()
    {
    }
    
    /// <summary>
    /// Gets the binding id.
    /// </summary>
    public abstract string BindingId { get; }

    /// <inheritdoc />
    public abstract IEnumerator<object> GetEnumerator();

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}