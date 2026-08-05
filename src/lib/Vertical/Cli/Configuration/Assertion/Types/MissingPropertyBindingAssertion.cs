using System.Reflection;

namespace Vertical.Cli.Configuration.Assertion.Types;

/// <summary>
/// Indicates the property of a model isn't bound.
/// </summary>
public sealed class MissingPropertyBindingAssertion : ConfigurationAssertion
{
    /// <inheritdoc />
    public MissingPropertyBindingAssertion(Type modelType, PropertyInfo property)
    {
        ModelType = modelType;
        Property = property;
    }

    /// <summary>
    /// Gets the model type.
    /// </summary>
    public Type ModelType { get; }
    
    /// <summary>
    /// Gets the property that is missing a binding.
    /// </summary>
    public PropertyInfo Property { get; }

    /// <inheritdoc />
    public override string GroupingKey => KeyHelpers.Create(ModelType);

    /// <inheritdoc />
    public override string GetIssueDescription()
    {
        return $"Property {Property.PropertyType} '{Property.Name}' has no symbol or binding defined";
    }
}