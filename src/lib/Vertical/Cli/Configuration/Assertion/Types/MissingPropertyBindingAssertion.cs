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

    public Type ModelType { get; }
    public PropertyInfo Property { get; }

    /// <inheritdoc />
    public override string GroupingKey => KeyHelpers.Create(ModelType);

    /// <inheritdoc />
    public override string GetIssueDescription()
    {
        return $"Property {Property.PropertyType} '{Property.Name}' has no symbol or binding defined";
    }
}