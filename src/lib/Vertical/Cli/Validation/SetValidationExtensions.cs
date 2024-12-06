namespace Vertical.Cli.Validation;

/// <summary>
/// Extensions involving sets.
/// </summary>
public static class SetValidationExtensions
{
    /// <summary>
    /// Adds a rule that the provided value must be contained in a set.
    /// </summary>
    /// <param name="instance">Builder instance</param>
    /// <param name="values">The set the value must be in.</param>
    /// <param name="comparer">Comparer implementation</param>
    /// <param name="message">The message to report if the validation fails</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">Comparable value type</typeparam>
    /// <returns>A reference to this instance</returns>
    public static ValidationBuilder<TModel, TValue> In<TModel, TValue>(
        this ValidationBuilder<TModel, TValue> instance,
        IReadOnlyCollection<TValue> values,
        IEqualityComparer<TValue>? comparer = null,
        string? message = null) where TModel : class
    {
        return instance.Must((_, value) => values.Contains(value, comparer),
            message ?? $"must be one of [{string.Join(',', values)}]");
    }
    
    /// <summary>
    /// Adds a rule that the provided value cannot be contained in a set.
    /// </summary>
    /// <param name="instance">Builder instance</param>
    /// <param name="values">The set the value must be in.</param>
    /// <param name="comparer">Comparer implementation</param>
    /// <param name="message">The message to report if the validation fails</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">Comparable value type</typeparam>
    /// <returns>A reference to this instance</returns>
    public static ValidationBuilder<TModel, TValue> NotIn<TModel, TValue>(
        this ValidationBuilder<TModel, TValue> instance,
        IReadOnlyCollection<TValue> values,
        IEqualityComparer<TValue>? comparer = null,
        string? message = null) where TModel : class
    {
        return instance.Must((_, value) => !values.Contains(value, comparer),
            message ?? $"cannot be one of [{string.Join(',', values)}]");
    }
    
    /// <summary>
    /// Adds a rule that the provided value must be contained in a set.
    /// </summary>
    /// <param name="instance">Builder instance</param>
    /// <param name="values">The set the value must be in.</param>
    /// <param name="comparer">Comparer implementation</param>
    /// <param name="message">The message to report if the validation fails</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">Comparable value type</typeparam>
    /// <returns>A reference to this instance</returns>
    public static ValidationBuilder<TModel, TValue?> InOrNull<TModel, TValue>(
        this ValidationBuilder<TModel, TValue?> instance,
        IReadOnlyCollection<TValue> values,
        IEqualityComparer<TValue>? comparer = null,
        string? message = null) where TModel : class
    {
        return instance.Must((_, value) => value is null || values.Contains(value, comparer),
            message ?? $"must be one of [{string.Join(',', values)}]");
    }
    
    /// <summary>
    /// Adds a rule that the provided value cannot be contained in a set.
    /// </summary>
    /// <param name="instance">Builder instance</param>
    /// <param name="values">The set the value must be in.</param>
    /// <param name="comparer">Comparer implementation</param>
    /// <param name="message">The message to report if the validation fails</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">Comparable value type</typeparam>
    /// <returns>A reference to this instance</returns>
    public static ValidationBuilder<TModel, TValue?> NotInOrNull<TModel, TValue>(
        this ValidationBuilder<TModel, TValue?> instance,
        IReadOnlyCollection<TValue> values,
        IEqualityComparer<TValue>? comparer = null,
        string? message = null) where TModel : class
    {
        return instance.Must((_, value) => value is null || !values.Contains(value, comparer),
            message ?? $"cannot be one of [{string.Join(',', values)}]");
    }
}