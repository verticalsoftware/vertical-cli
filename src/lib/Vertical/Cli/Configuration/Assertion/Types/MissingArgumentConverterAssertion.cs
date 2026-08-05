namespace Vertical.Cli.Configuration.Assertion.Types;

/// <summary>
/// Indicates an argument converter is missing for a type detected on a model property.
/// </summary>
public sealed class MissingArgumentConverterAssertion : ConfigurationAssertion
{
    /// <inheritdoc />
    internal MissingArgumentConverterAssertion(Type type)
    {
        Type = type;
    }

    /// <summary>
    /// Gets the type without a converter.
    /// </summary>
    public Type Type { get; }

    /// <inheritdoc />
    public override string GroupingKey => KeyHelpers.Conversion;

    /// <inheritdoc />
    public override string GetIssueDescription()
    {
        return $"Argument converter for type {Type} not found";
    }
}