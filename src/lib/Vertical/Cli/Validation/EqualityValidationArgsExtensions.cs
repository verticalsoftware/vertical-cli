namespace Vertical.Cli.Validation;

public static partial class ValidationArgsExtensions
{
    /// <summary>
    /// Adds a validation action that reports an error if the client value is equals the provided value.
    /// </summary>
    /// <param name="context"><see cref="ValidationContext{TModel,TValue}"/></param>
    /// <param name="value">The value to compare the client value with.</param>
    /// <param name="messageProvider">A function that provides a custom message when the validation fails.</param>
    /// <param name="comparer">The equality comparer instance to use.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <returns>A reference to this provided <see cref="ValidationContext{TModel,TValue}"/> instance.</returns>
    public static ValidationContext<TModel, TValue> NotEqualTo<TModel, TValue>(
        this ValidationContext<TModel, TValue> context,
        TValue value,
        Func<TValue, string>? messageProvider = null,
        IEqualityComparer<TValue>? comparer = null)
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(context);
        comparer ??= EqualityComparer<TValue>.Default;

        if (comparer.Equals(context.Value, value))
            return context;
        
        context.AddValidationError(messageProvider?.Invoke(context.Value) ?? $"value must not be equal to {value}");
        return context;
    }
    
    /// <summary>
    /// Adds a validation action that reports an error if the client value is equals the provided value.
    /// </summary>
    /// <param name="context"><see cref="ValidationContext{TModel,TValue}"/></param>
    /// <param name="value">The value to compare the client value with.</param>
    /// <param name="messageProvider">A function that provides a custom message when the validation fails.</param>
    /// <param name="comparer">The equality comparer instance to use.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <returns>A reference to this provided <see cref="ValidationContext{TModel,TValue}"/> instance.</returns>
    public static ValidationContext<TModel, TValue?> NotEqualToOrNull<TModel, TValue>(
        this ValidationContext<TModel, TValue?> context,
        TValue value,
        Func<TValue, string>? messageProvider = null,
        IEqualityComparer<TValue>? comparer = null)
        where TModel : class
        where TValue : class
    {
        ArgumentNullException.ThrowIfNull(context);
        comparer ??= EqualityComparer<TValue>.Default;

        if (context.Value is null || comparer.Equals(context.Value, value))
            return context;
        
        context.AddValidationError(messageProvider?.Invoke(context.Value) ?? $"value must not be equal to {value} or omitted entirely");
        return context;
    }
    
    /// <summary>
    /// Adds a validation action that reports an error if the client value is not in the given set.
    /// </summary>
    /// <param name="context"><see cref="ValidationContext{TModel,TValue}"/></param>
    /// <param name="valueSet">The hash set that contains acceptable values.</param>
    /// <param name="messageProvider">A function that provides a custom message when the validation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <returns>A reference to this provided <see cref="ValidationContext{TModel,TValue}"/> instance.</returns>
    public static ValidationContext<TModel, TValue> MemberOf<TModel, TValue>(
        this ValidationContext<TModel, TValue> context,
        ISet<TValue> valueSet,
        Func<TValue, string>? messageProvider = null)
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(valueSet);
        
        if (valueSet.Contains(context.Value))
            return context;

        var setValues = string.Join(", ", valueSet);
        context.AddValidationError(messageProvider?.Invoke(context.Value) ?? $"value must be one of [{setValues}]");
        return context;
    }
    
    /// <summary>
    /// Adds a validation action that reports an error if the client value is not null and not in the given set.
    /// </summary>
    /// <param name="context"><see cref="ValidationContext{TModel,TValue}"/></param>
    /// <param name="valueSet">The hash set that contains acceptable values.</param>
    /// <param name="messageProvider">A function that provides a custom message when the validation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <returns>A reference to this provided <see cref="ValidationContext{TModel,TValue}"/> instance.</returns>
    public static ValidationContext<TModel, TValue?> MemberOfOrNull<TModel, TValue>(
        this ValidationContext<TModel, TValue?> context,
        ISet<TValue> valueSet,
        Func<TValue, string>? messageProvider = null)
        where TModel : class
        where TValue : class
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(valueSet);
        
        if (context.Value is null || valueSet.Contains(context.Value))
            return context;

        var setValues = string.Join(", ", valueSet);
        context.AddValidationError(messageProvider?.Invoke(context.Value) ?? $"value must be one of [{setValues}] or omitted entirely");
        return context;
    }
}