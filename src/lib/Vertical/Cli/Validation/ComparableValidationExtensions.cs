namespace Vertical.Cli.Validation;

/// <summary>
/// Extensions for types that implement <see cref="IComparable{T}"/>
/// </summary>
public static class ComparableValidationExtensions
{
    /// <summary>
    /// Adds a rule that the provided value must be greater than a minimum value.
    /// </summary>
    /// <param name="instance">Builder instance</param>
    /// <param name="min">Minimum value</param>
    /// <param name="comparer">Comparer implementation</param>
    /// <param name="message">The message to report if the validation fails</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">Comparable value type</typeparam>
    /// <returns>A reference to this instance</returns>
    public static ValidationBuilder<TModel, TValue> GreaterThan<TModel, TValue>(
        this ValidationBuilder<TModel, TValue> instance,
        TValue min,
        IComparer<TValue>? comparer = null,
        string? message = null)
        where TModel : class
        where TValue : IComparable<TValue>
    {
        return instance.Must((_, value) => (comparer ?? Comparer<TValue>.Default).Compare(value, min) > 0,
            message ?? $"must be greater than {min}");
    }
    
    /// <summary>
    /// Adds a rule that the provided value must be greater or equal to a minimum value.
    /// </summary>
    /// <param name="instance">Builder instance</param>
    /// <param name="min">Minimum value</param>
    /// <param name="comparer">Comparer implementation</param>
    /// <param name="message">The message to report if the validation fails</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">Comparable value type</typeparam>
    /// <returns>A reference to this instance</returns>
    public static ValidationBuilder<TModel, TValue> GreaterThanOrEqual<TModel, TValue>(
        this ValidationBuilder<TModel, TValue> instance,
        TValue min,
        IComparer<TValue>? comparer = null,
        string? message = null)
        where TModel : class
        where TValue : IComparable<TValue>
    {
        return instance.Must((_, value) => (comparer ?? Comparer<TValue>.Default).Compare(value, min) >= 0,
            message ?? $"must be greater or equal to {min}");
    }
    
    /// <summary>
    /// Adds a rule that the provided value must be less than a maximum value.
    /// </summary>
    /// <param name="instance">Builder instance</param>
    /// <param name="max">Maximum value</param>
    /// <param name="comparer">Comparer implementation</param>
    /// <param name="message">The message to report if the validation fails</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">Comparable value type</typeparam>
    /// <returns>A reference to this instance</returns>
    public static ValidationBuilder<TModel, TValue> LessThan<TModel, TValue>(
        this ValidationBuilder<TModel, TValue> instance,
        TValue max,
        IComparer<TValue>? comparer = null,
        string? message = null)
        where TModel : class
        where TValue : IComparable<TValue>
    {
        return instance.Must((_, value) => (comparer ?? Comparer<TValue>.Default).Compare(value, max) < 0,
            message ?? $"must be less than {max}");
    }
    
    /// <summary>
    /// Adds a rule that the provided value must be less or equal to a maximum value.
    /// </summary>
    /// <param name="instance">Builder instance</param>
    /// <param name="max">Maximum value</param>
    /// <param name="comparer">Comparer implementation</param>
    /// <param name="message">The message to report if the validation fails</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">Comparable value type</typeparam>
    /// <returns>A reference to this instance</returns>
    public static ValidationBuilder<TModel, TValue> LessThanOrEqual<TModel, TValue>(
        this ValidationBuilder<TModel, TValue> instance,
        TValue max,
        IComparer<TValue>? comparer = null,
        string? message = null)
        where TModel : class
        where TValue : IComparable<TValue>
    {
        return instance.Must((_, value) => (comparer ?? Comparer<TValue>.Default).Compare(value, max) <= 0,
            message ?? $"must be less or equal to {max}");
    }
    
    /// <summary>
    /// Adds a rule that the provided value must be inclusively between two values.
    /// </summary>
    /// <param name="instance">Builder instance</param>
    /// <param name="min">Minimum value</param>
    /// <param name="max">Maximum value</param>
    /// <param name="comparer">Comparer implementation</param>
    /// <param name="message">The message to report if the validation fails</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">Comparable value type</typeparam>
    /// <returns>A reference to this instance</returns>
    public static ValidationBuilder<TModel, TValue> BetweenInclusive<TModel, TValue>(
        this ValidationBuilder<TModel, TValue> instance,
        TValue min,
        TValue max,
        IComparer<TValue>? comparer = null,
        string? message = null)
        where TModel : class
        where TValue : IComparable<TValue>
    {
        var actualComparer = comparer ?? Comparer<TValue>.Default;
        return instance.Must((_, value) => actualComparer.Compare(value, min) >= 0 
                                           && actualComparer.Compare(value, max) <= 0,
            message ?? $"must be between {min} and {max} (inclusive)");
    }
    
    /// <summary>
    /// Adds a rule that the provided value must be exclusively between two values.
    /// </summary>
    /// <param name="instance">Builder instance</param>
    /// <param name="min">Minimum value</param>
    /// <param name="max">Maximum value</param>
    /// <param name="comparer">Comparer implementation</param>
    /// <param name="message">The message to report if the validation fails</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">Comparable value type</typeparam>
    /// <returns>A reference to this instance</returns>
    public static ValidationBuilder<TModel, TValue> BetweenExclusive<TModel, TValue>(
        this ValidationBuilder<TModel, TValue> instance,
        TValue min,
        TValue max,
        IComparer<TValue>? comparer = null,
        string? message = null)
        where TModel : class
        where TValue : IComparable<TValue>
    {
        var actualComparer = comparer ?? Comparer<TValue>.Default;
        return instance.Must((_, value) => actualComparer.Compare(value, min) > 0 
                                           && actualComparer.Compare(value, max) < 0,
            message ?? $"must be between {min} and {max} (exclusive)");
    }
    
    /// <summary>
    /// Adds a rule that the provided value must be greater than a minimum value.
    /// </summary>
    /// <param name="instance">Builder instance</param>
    /// <param name="min">Minimum value</param>
    /// <param name="comparer">Comparer implementation</param>
    /// <param name="message">The message to report if the validation fails</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">Comparable value type</typeparam>
    /// <returns>A reference to this instance</returns>
    public static ValidationBuilder<TModel, TValue?> GreaterThanOrNull<TModel, TValue>(
        this ValidationBuilder<TModel, TValue?> instance,
        TValue min,
        IComparer<TValue>? comparer = null,
        string? message = null)
        where TModel : class
        where TValue : IComparable<TValue>
    {
        return instance.Must((_, value) => value is null || (comparer ?? Comparer<TValue>.Default).Compare(value, min) > 0,
            message ?? $"must be greater than {min}");
    }
    
    /// <summary>
    /// Adds a rule that the provided value must be greater or equal to a minimum value.
    /// </summary>
    /// <param name="instance">Builder instance</param>
    /// <param name="min">Minimum value</param>
    /// <param name="comparer">Comparer implementation</param>
    /// <param name="message">The message to report if the validation fails</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">Comparable value type</typeparam>
    /// <returns>A reference to this instance</returns>
    public static ValidationBuilder<TModel, TValue?> GreaterThanEqualOrNull<TModel, TValue>(
        this ValidationBuilder<TModel, TValue?> instance,
        TValue min,
        IComparer<TValue>? comparer = null,
        string? message = null)
        where TModel : class
        where TValue : IComparable<TValue>
    {
        return instance.Must((_, value) => value is null || (comparer ?? Comparer<TValue>.Default).Compare(value, min) >= 0,
            message ?? $"must be greater or equal to {min}");
    }
    
    /// <summary>
    /// Adds a rule that the provided value must be less than a maximum value.
    /// </summary>
    /// <param name="instance">Builder instance</param>
    /// <param name="max">Maximum value</param>
    /// <param name="comparer">Comparer implementation</param>
    /// <param name="message">The message to report if the validation fails</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">Comparable value type</typeparam>
    /// <returns>A reference to this instance</returns>
    public static ValidationBuilder<TModel, TValue?> LessThanOrNUll<TModel, TValue>(
        this ValidationBuilder<TModel, TValue?> instance,
        TValue max,
        IComparer<TValue>? comparer = null,
        string? message = null)
        where TModel : class
        where TValue : IComparable<TValue>
    {
        return instance.Must((_, value) => value is null || (comparer ?? Comparer<TValue>.Default).Compare(value, max) < 0,
            message ?? $"must be less than {max}");
    }
    
    /// <summary>
    /// Adds a rule that the provided value must be less or equal to a maximum value.
    /// </summary>
    /// <param name="instance">Builder instance</param>
    /// <param name="max">Maximum value</param>
    /// <param name="comparer">Comparer implementation</param>
    /// <param name="message">The message to report if the validation fails</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">Comparable value type</typeparam>
    /// <returns>A reference to this instance</returns>
    public static ValidationBuilder<TModel, TValue?> LessThanEqualOrNull<TModel, TValue>(
        this ValidationBuilder<TModel, TValue?> instance,
        TValue max,
        IComparer<TValue>? comparer = null,
        string? message = null)
        where TModel : class
        where TValue : IComparable<TValue>
    {
        return instance.Must((_, value) => value is null || (comparer ?? Comparer<TValue>.Default).Compare(value, max) <= 0,
            message ?? $"must be less or equal to {max}");
    }
    
    /// <summary>
    /// Adds a rule that the provided value must be inclusively between two values.
    /// </summary>
    /// <param name="instance">Builder instance</param>
    /// <param name="min">Minimum value</param>
    /// <param name="max">Maximum value</param>
    /// <param name="comparer">Comparer implementation</param>
    /// <param name="message">The message to report if the validation fails</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">Comparable value type</typeparam>
    /// <returns>A reference to this instance</returns>
    public static ValidationBuilder<TModel, TValue?> BetweenInclusiveOrNull<TModel, TValue>(
        this ValidationBuilder<TModel, TValue?> instance,
        TValue min,
        TValue max,
        IComparer<TValue>? comparer = null,
        string? message = null)
        where TModel : class
        where TValue : IComparable<TValue>
    {
        var actualComparer = comparer ?? Comparer<TValue>.Default;
        return instance.Must((_, value) => value is null || (actualComparer.Compare(value, min) >= 0
                                                             && actualComparer.Compare(value, max) <= 0),
            message ?? $"must be between {min} and {max} (inclusive)");
    }
    
    /// <summary>
    /// Adds a rule that the provided value must be exclusively between two values.
    /// </summary>
    /// <param name="instance">Builder instance</param>
    /// <param name="min">Minimum value</param>
    /// <param name="max">Maximum value</param>
    /// <param name="comparer">Comparer implementation</param>
    /// <param name="message">The message to report if the validation fails</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">Comparable value type</typeparam>
    /// <returns>A reference to this instance</returns>
    public static ValidationBuilder<TModel, TValue?> BetweenExclusiveOrNull<TModel, TValue>(
        this ValidationBuilder<TModel, TValue?> instance,
        TValue min,
        TValue max,
        IComparer<TValue>? comparer = null,
        string? message = null)
        where TModel : class
        where TValue : IComparable<TValue>
    {
        var actualComparer = comparer ?? Comparer<TValue>.Default;
        return instance.Must((_, value) => value is null || (actualComparer.Compare(value, min) > 0 
                                                             && actualComparer.Compare(value, max) < 0),
            message ?? $"must be between {min} and {max} (exclusive)");
    }
}