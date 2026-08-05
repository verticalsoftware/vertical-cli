namespace Vertical.Cli.Configuration.Assertion.Types;

/// <summary>
/// Indicates a model type is expected by a command handler, but a binder for the type
/// was not configured.
/// </summary>
public sealed class MissingModelBinderAssertion : ConfigurationAssertion
{
    /// <inheritdoc />
    public MissingModelBinderAssertion(Type modelType)
    {
        ModelType = modelType;
    }

    /// <summary>
    /// Gets the model type that has no binder defined.
    /// </summary>
    public Type ModelType { get; }

    /// <inheritdoc />
    public override string GroupingKey => KeyHelpers.Binding;

    /// <inheritdoc />
    public override string GetIssueDescription()
    {
        return $"No binder defined for model {ModelType.FullName}";
    }
}