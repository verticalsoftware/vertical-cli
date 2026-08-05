using Vertical.Cli.Binding;

namespace Vertical.Cli.Configuration.Assertion.Types;

/// <summary>
/// Indicates a model property was bound more than once.
/// </summary>
public sealed class DuplicatePropertyBindingAssertion : ConfigurationAssertion
{
    internal DuplicatePropertyBindingAssertion(Type modelType, 
        string propertyName, 
        IBindingSource[] bindingSources)
    {
        ModelType = modelType;
        PropertyName = propertyName;
        BindingSources = bindingSources;
    }

    /// <summary>
    /// Gets the model type.
    /// </summary>
    public Type ModelType { get; }
    
    /// <summary>
    /// Gets the binding property name.
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    /// Gets the sources bound to the property.
    /// </summary>
    public IBindingSource[] BindingSources { get; }

    /// <inheritdoc />
    public override string GroupingKey => KeyHelpers.Create(ModelType);

    /// <inheritdoc />
    public override string GetIssueDescription()
    {
        return $"Property {ModelType}.{PropertyName}  bound multiple times:";
    }

    /// <inheritdoc />
    public override IEnumerable<string> GetIssueDetail()
    {
        return BindingSources.Select(source => $"{source}");
    }
}