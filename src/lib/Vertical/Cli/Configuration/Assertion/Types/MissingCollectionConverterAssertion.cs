namespace Vertical.Cli.Configuration.Assertion.Types;


/// <summary>
/// Indicates a collection converter is missing for a collection type detected on a model property.
/// </summary>
public sealed class MissingCollectionConverterAssertion : ConfigurationAssertion
{
    internal MissingCollectionConverterAssertion(Type elementType, Type collectionType)
    {
        ElementType = elementType;
        CollectionType = collectionType;
    }

    /// <summary>
    /// Gets the collection element type.
    /// </summary>
    public Type ElementType { get; }

    /// <summary>
    /// Gets the collection type.
    /// </summary>
    public Type CollectionType { get; }
    
    /// <inheritdoc />
    public override string GroupingKey => KeyHelpers.Conversion;

    /// <inheritdoc />
    public override string GetIssueDescription()
    {
        return $"Collection converter for IEnumerable<{ElementType}> -> {CollectionType} not found";
    }
}