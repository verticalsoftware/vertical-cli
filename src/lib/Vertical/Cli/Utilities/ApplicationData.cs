namespace Vertical.Cli.Utilities;

/// <summary>
/// Represents an options manager for the application.
/// </summary>
public sealed class ApplicationData
{
    private enum EntryType
    {
        Value,
        Configured
    };
    
    private readonly Dictionary<(EntryType, Type), object> _data = [];

    /// <summary>
    /// Gets a single value of the given type.
    /// </summary>
    /// <param name="defaultValue">The value to return when an entry of the given type is not found.</param>
    /// <typeparam name="T">The value type.</typeparam>
    /// <returns>The previously set value, or <paramref name="defaultValue"/>.</returns>
    public T GetValueOrDefault<T>(T defaultValue) where T : notnull
    {
        return (T)_data.GetValueOrDefault((EntryType.Value, typeof(T)), defaultValue);
    }

    /// <summary>
    /// Sets a single value of the given type.
    /// </summary>
    /// <param name="value">The value to set.</param>
    /// <typeparam name="T">The value type.</typeparam>
    public void SetValue<T>(T value) where T : notnull
    {
        _data[(EntryType.Value, typeof(T))] = value;
    }

    /// <summary>
    /// Gets or creates an instance of <typeparamref name="TOptions"/>, then invokes
    /// the provided delegate with the instance if not <c>null</c>.
    /// </summary>
    /// <param name="configure">An action that manipulates an options object.</param>
    /// <typeparam name="TOptions">Options type</typeparam>
    /// <returns>The current or newly created options instance.</returns>
    public TOptions Configure<TOptions>(Action<TOptions>? configure = null) where TOptions : class, new()
    {
        var options = (TOptions)_data.GetOrAdd(
            (EntryType.Configured, typeof(TOptions)),
            () => new TOptions());
        
        configure?.Invoke(options);
        return options;
    }
}