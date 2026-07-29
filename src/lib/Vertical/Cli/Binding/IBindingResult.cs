using Vertical.Cli.Diagnostics;

namespace Vertical.Cli.Binding;

/// <summary>
/// Represents a binding result.
/// </summary>
public interface IBindingResult
{
    /// <summary>
    /// Gets the value type.
    /// </summary>
    Type ValueType { get; }
    
    /// <summary>
    /// Gets the error that occurred during value processing.
    /// </summary>
    CommandLineError? Error { get; }
    
    /// <summary>
    /// Gets the binding name.
    /// </summary>
    string BindingName { get; }
}