namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents a simple dictionary applications can use to pass data within the
/// processing elements of the application.
/// </summary>
public sealed class PropertyBag
{
    private readonly Dictionary<Type, object> _data = [];

    /// <summary>
    /// Gets the application data of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of data to retrieve.</typeparam>
    /// <returns>A reference to the object data or <c>null</c>.</returns>
    public T? GetData<T>()
    {
        return (T?)_data.GetValueOrDefault(typeof(T));
    }

    /// <summary>
    /// Gets or creates an application data object.
    /// </summary>
    /// <param name="factory">A function that creates the object if it isn't found.</param>
    /// <typeparam name="T">The type of data to get or create.</typeparam>
    /// <returns>A reference to the object data.</returns>
    public T GetOrCreateData<T>(Func<T> factory) where T : notnull
    {
        if (_data.TryGetValue(typeof(T), out var obj))
            return (T)obj;
        
        var newInstance = factory();
        _data.Add(typeof(T), newInstance);
        return newInstance;
    }

    /// <summary>
    /// Gets the application data of the specified type.
    /// </summary>
    /// <param name="defaultValue">The value to return if the data object is not found.</param>
    /// <typeparam name="T">The type of data to retrieve.</typeparam>
    /// <returns>The stored value or the default value.</returns>
    public T GetValueOrDefault<T>(T defaultValue)
    {
        return TryGetData<T>(out var value) ? value : defaultValue;
    }

    /// <summary>
    /// Tries to get an application data object.
    /// </summary>
    /// <param name="value">When found in the bag, the stored value.</param>
    /// <typeparam name="T">The data type</typeparam>
    /// <returns><c>true</c> if the data object was found.</returns>
    public bool TryGetData<T>(out T value)
    {
        var hasValue = _data.TryGetValue(typeof(T), out var obj);
        value = hasValue ? (T)obj! : default!;
        return hasValue;
    }

    /// <summary>
    /// Removes a data object.
    /// </summary>
    /// <typeparam name="T">The type of data to remove.</typeparam>
    /// <returns>The data object or <c>null</c> if it was not present in the bag.</returns>
    public T? RemoveData<T>()
    {
        if (!_data.TryGetValue(typeof(T), out var obj)) return default;
        
        _data.Remove(typeof(T));
        return (T?)obj;
    }
    
    /// <summary>
    /// Sets application data.
    /// </summary>
    /// <param name="obj">The data object.</param>
    /// <typeparam name="T">The type of data object.</typeparam>
    public void SetData<T>(T obj)
    {
        _data[typeof(T)] = obj!;
    }
}