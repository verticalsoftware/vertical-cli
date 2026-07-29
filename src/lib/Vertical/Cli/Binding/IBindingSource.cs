namespace Vertical.Cli.Binding;

/// <summary>
/// Represents an object that can use application arguments or other configuration data
/// to provide binding values.
/// </summary>
public interface IBindingSource
{
    /// <summary>
    /// Gets the binding model type.
    /// </summary>
    Type ValueType { get; }
    
    /// <summary>
    /// Gets the property binding name.
    /// </summary>
    string BindingName { get; }
    
    /// <summary>
    /// When implemented by a class, creates a property binder.
    /// </summary>
    /// <returns><see cref="PropertyBinder"/></returns>
    PropertyBinder CreatePropertyBinder();
}