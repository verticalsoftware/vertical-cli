using Vertical.Cli.Configuration;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Binding;

/// <summary>
/// Base type for argument bindings.
/// </summary>
public abstract class ArgumentBinding
{
    internal ArgumentBinding()
    {
    }
    
    /// <summary>
    /// Gets the binding id.
    /// </summary>
    public abstract string BindingId { get; }
}