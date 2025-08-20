namespace Vertical.Cli.Validation;

/// <summary>
/// Extensions for <see cref="ValidationContext{TModel,TValue}"/>
/// </summary>
public static partial class ValidationArgsExtensions
{
    /// <summary>
    /// Adds a validation action that reports an error if the client value is greater
    /// than the given value.
    /// </summary>
    /// <param name="context"><see cref="ValidationContext{TModel,TValue}"/></param>
    /// <param name="value">The value to compare the client value with.</param>
    /// <param name="messageProvider">A function that provides a custom message when the validation fails.</param>
    /// <param name="comparer">The comparer instance to use.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <returns>A reference to this provided <see cref="ValidationContext{TModel,TValue}"/> instance.</returns>
    public static ValidationContext<TModel, TValue> LessThan<TModel, TValue>(
        this ValidationContext<TModel, TValue> context,
        TValue value,
        Func<TValue, string>? messageProvider = null,
        IComparer<TValue>? comparer = null)
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(context);
        comparer ??= Comparer<TValue>.Default;

        if (comparer.Compare(context.Value, value) < 0)
            return context;
        
        context.AddValidationError(messageProvider?.Invoke(context.Value) ?? $"value must be less than {value}");
        return context;
    }
    
    /// <summary>
    /// Adds a validation action that reports an error if the client value is not null and greater
    /// than the given value.
    /// </summary>
    /// <param name="context"><see cref="ValidationContext{TModel,TValue}"/></param>
    /// <param name="value">The value to compare the client value with.</param>
    /// <param name="messageProvider">A function that provides a custom message when the validation fails.</param>
    /// <param name="comparer">The comparer instance to use.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <returns>A reference to this provided <see cref="ValidationContext{TModel,TValue}"/> instance.</returns>
    public static ValidationContext<TModel, TValue?> LessThanOrNull<TModel, TValue>(
        this ValidationContext<TModel, TValue?> context,
        TValue value,
        Func<TValue, string>? messageProvider = null,
        IComparer<TValue>? comparer = null)
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(context);
        comparer ??= Comparer<TValue>.Default;

        if (context.Value is null || comparer.Compare(context.Value, value) < 0)
            return context;
        
        context.AddValidationError(messageProvider?.Invoke(context.Value) ?? $"value must be less than {value} or omitted entirely");
        return context;
    }
    
    /// <summary>
    /// Adds a validation action that reports an error if the client value is greater
    /// than the given value.
    /// </summary>
    /// <param name="context"><see cref="ValidationContext{TModel,TValue}"/></param>
    /// <param name="value">The value to compare the client value with.</param>
    /// <param name="messageProvider">A function that provides a custom message when the validation fails.</param>
    /// <param name="comparer">The comparer instance to use.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <returns>A reference to this provided <see cref="ValidationContext{TModel,TValue}"/> instance.</returns>
    public static ValidationContext<TModel, TValue> LessOrEqual<TModel, TValue>(
        this ValidationContext<TModel, TValue> context,
        TValue value,
        Func<TValue, string>? messageProvider = null,
        IComparer<TValue>? comparer = null)
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(context);
        comparer ??= Comparer<TValue>.Default;

        if (comparer.Compare(context.Value, value) <= 0)
            return context;
        
        context.AddValidationError(messageProvider?.Invoke(context.Value) ?? $"value must be less or equal to {value}");
        return context;
    }
    
    /// <summary>
    /// Adds a validation action that reports an error if the client value is not null and greater
    /// than the given value.
    /// </summary>
    /// <param name="context"><see cref="ValidationContext{TModel,TValue}"/></param>
    /// <param name="value">The value to compare the client value with.</param>
    /// <param name="messageProvider">A function that provides a custom message when the validation fails.</param>
    /// <param name="comparer">The comparer instance to use.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <returns>A reference to this provided <see cref="ValidationContext{TModel,TValue}"/> instance.</returns>
    public static ValidationContext<TModel, TValue?> LessEqualOrNull<TModel, TValue>(
        this ValidationContext<TModel, TValue?> context,
        TValue value,
        Func<TValue, string>? messageProvider = null,
        IComparer<TValue>? comparer = null)
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(context);
        comparer ??= Comparer<TValue>.Default;

        if (context.Value is null || comparer.Compare(context.Value, value) <= 0)
            return context;
        
        context.AddValidationError(messageProvider?.Invoke(context.Value) ?? $"value must be less or equal to {value} or omitted entirely");
        return context;
    }
    
    /// <summary>
    /// Adds a validation action that reports an error if the client value is less
    /// than the given value.
    /// </summary>
    /// <param name="context"><see cref="ValidationContext{TModel,TValue}"/></param>
    /// <param name="value">The value to compare the client value with.</param>
    /// <param name="messageProvider">A function that provides a custom message when the validation fails.</param>
    /// <param name="comparer">The comparer instance to use.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <returns>A reference to this provided <see cref="ValidationContext{TModel,TValue}"/> instance.</returns>
    public static ValidationContext<TModel, TValue> GreaterThan<TModel, TValue>(
        this ValidationContext<TModel, TValue> context,
        TValue value,
        Func<TValue, string>? messageProvider = null,
        IComparer<TValue>? comparer = null)
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(context);
        comparer ??= Comparer<TValue>.Default;

        if (comparer.Compare(context.Value, value) > 0)
            return context;
        
        context.AddValidationError(messageProvider?.Invoke(context.Value) ?? $"value must be greater than {value}");
        return context;
    }
    
    /// <summary>
    /// Adds a validation action that reports an error if the client value not null and less
    /// than the given value.
    /// </summary>
    /// <param name="context"><see cref="ValidationContext{TModel,TValue}"/></param>
    /// <param name="value">The value to compare the client value with.</param>
    /// <param name="messageProvider">A function that provides a custom message when the validation fails.</param>
    /// <param name="comparer">The comparer instance to use.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <returns>A reference to this provided <see cref="ValidationContext{TModel,TValue}"/> instance.</returns>
    public static ValidationContext<TModel, TValue?> GreaterThanOrNull<TModel, TValue>(
        this ValidationContext<TModel, TValue?> context,
        TValue value,
        Func<TValue, string>? messageProvider = null,
        IComparer<TValue>? comparer = null)
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(context);
        comparer ??= Comparer<TValue>.Default;

        if (context.Value is null || comparer.Compare(context.Value, value) > 0)
            return context;
        
        context.AddValidationError(messageProvider?.Invoke(context.Value) ?? $"value must be greater than {value} or omitted entirely");
        return context;
    }
    
    /// <summary>
    /// Adds a validation action that reports an error if the client value is less or equal
    /// to the given value.
    /// </summary>
    /// <param name="context"><see cref="ValidationContext{TModel,TValue}"/></param>
    /// <param name="value">The value to compare the client value with.</param>
    /// <param name="messageProvider">A function that provides a custom message when the validation fails.</param>
    /// <param name="comparer">The comparer instance to use.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <returns>A reference to this provided <see cref="ValidationContext{TModel,TValue}"/> instance.</returns>
    public static ValidationContext<TModel, TValue> GreaterOrEqual<TModel, TValue>(
        this ValidationContext<TModel, TValue> context,
        TValue value,
        Func<TValue, string>? messageProvider = null,
        IComparer<TValue>? comparer = null)
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(context);
        comparer ??= Comparer<TValue>.Default;

        if (comparer.Compare(context.Value, value) >= 0)
            return context;
        
        context.AddValidationError(messageProvider?.Invoke(context.Value) ?? $"value must be greater or equal to {value}");
        return context;
    }
}