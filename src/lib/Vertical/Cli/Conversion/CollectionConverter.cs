namespace Vertical.Cli.Conversion;

/// <summary>
/// Defines a delegate that converts an enumeration of values to a collection type.
/// </summary>
/// <typeparam name="TElement">Element type</typeparam>
/// <typeparam name="TCollection">Collection type</typeparam>
public delegate TCollection CollectionConverter<in TElement, out TCollection>(IEnumerable<TElement> values)
    where TCollection : IEnumerable<TElement>;