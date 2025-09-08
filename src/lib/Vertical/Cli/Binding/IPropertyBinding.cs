namespace Vertical.Cli.Binding;

/// <summary>
/// Represents the interface of an object that binds a property value in an option model.
/// </summary>
public interface IPropertyBinding
{
    /// <summary>
    /// Gets the name of the property being bound.
    /// </summary>
    string BindingName { get; }
    
    /// <summary>
    /// Gets the model type the property is part of.
    /// </summary>
    Type ModelType { get; }
    
    /// <summary>
    /// Gets the value type of the property.
    /// </summary>
    Type ValueType { get; }
    
    /// <summary>
    /// Gets whether the binding can provide a value.
    /// </summary>
    bool HasBindingOptions { get; }
}

/// <summary>
/// Represents the interface of an object that binds a property value in an option model.
/// </summary>
public interface IPropertyBinding<TValue> : IPropertyBinding
{
    /// <summary>
    /// Tries to bind a value using the given arguments.
    /// </summary>
    /// <param name="binder">The object that contains binding data.</param>
    /// <returns><c>true</c> if a value was set.</returns>
    bool TryBindValue(PropertyBinder<TValue> binder);
}