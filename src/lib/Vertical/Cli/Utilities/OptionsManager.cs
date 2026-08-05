namespace Vertical.Cli.Utilities;

/// <summary>
/// Represents an options manager for the application.
/// </summary>
public sealed class OptionsManager
{
    private readonly Dictionary<Type, object> _data = [];

    /// <summary>
    /// Gets whether the given options object has been created.
    /// </summary>
    /// <param name="type">Options type</param>
    /// <returns><c>true</c> if this instance contains an object of the given type.</returns>
    public bool Contains(Type type) => _data.ContainsKey(type);
    
    /// <summary>
    /// Gets or creates the options type.
    /// </summary>
    /// <typeparam name="TOptions">Options type</typeparam>
    /// <returns>A singleton instance of the options type.</returns>
    public TOptions GetOptions<TOptions>() where TOptions : class, new()
    {
        return (TOptions)_data.GetOrAdd(typeof(TOptions), () => new TOptions());
    }

    /// <summary>
    /// Invokes the given delegate for configuration on the specified options type.
    /// </summary>
    /// <param name="configure">An action that manipulates an options object.</param>
    /// <typeparam name="TOptions">Options type</typeparam>
    public void Configure<TOptions>(Action<TOptions> configure) where TOptions : class, new()
    {
        ArgumentNullException.ThrowIfNull(configure);
        configure(GetOptions<TOptions>());
    }
}