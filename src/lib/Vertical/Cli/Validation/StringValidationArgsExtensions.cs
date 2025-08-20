using System.Text.RegularExpressions;

namespace Vertical.Cli.Validation;

public static partial class ValidationArgsExtensions
{
    /// <summary>
    /// Adds a validation action that reports an error if the client provided value's length is less
    /// than the given string length.
    /// </summary>
    /// <param name="context"><see cref="ValidationContext{TModel,TValue}"/></param>
    /// <param name="length">The minimum length of the string.</param>
    /// <param name="messageProvider">A function that provides a custom message when the validation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this provided <see cref="ValidationContext{TModel,TValue}"/> instance.</returns>
    public static ValidationContext<TModel, string> MinimumLength<TModel>(
        this ValidationContext<TModel, string> context,
        int length,
        Func<string, string>? messageProvider = null) 
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentOutOfRangeException.ThrowIfLessThan(length, 0);
        
        if (context.Value.Length >= length)
            return context;
        
        context.AddValidationError(messageProvider?.Invoke(context.Value) 
                                ?? $"value must contain a minimum of {length} character(s)");
        return context;
    }
    
    /// <summary>
    /// Adds a validation action that reports an error if the client provided value's length is less
    /// than the given string length or is <c>null</c>
    /// </summary>
    /// <param name="context"><see cref="ValidationContext{TModel,TValue}"/></param>
    /// <param name="length">The minimum length of the string.</param>
    /// <param name="messageProvider">A function that provides a custom message when the validation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this provided <see cref="ValidationContext{TModel,TValue}"/> instance.</returns>
    public static ValidationContext<TModel, string?> MinimumLengthOrNull<TModel>(
        this ValidationContext<TModel, string?> context,
        int length,
        Func<string, string>? messageProvider = null) 
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentOutOfRangeException.ThrowIfLessThan(length, 0);

        
        if (context.Value is null || context.Value.Length >= length)
            return context;
        
        context.AddValidationError(messageProvider?.Invoke(context.Value) 
                                ?? $"value must contain a minimum of {length} character(s) or omitted entirely");
        return context;
    }
    
    /// <summary>
    /// Adds a validation action that reports an error if the client provided value's length is greater
    /// than the given string length.
    /// </summary>
    /// <param name="context"><see cref="ValidationContext{TModel,TValue}"/></param>
    /// <param name="length">The maximum length of the string.</param>
    /// <param name="messageProvider">A function that provides a custom message when the validation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this provided <see cref="ValidationContext{TModel,TValue}"/> instance.</returns>
    public static ValidationContext<TModel, string> MaximumLength<TModel>(
        this ValidationContext<TModel, string> context,
        int length,
        Func<string, string>? messageProvider = null) 
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentOutOfRangeException.ThrowIfLessThan(length, 0);

        if (context.Value.Length <= length)
            return context;
        
        context.AddValidationError(messageProvider?.Invoke(context.Value) 
                                ?? $"value must not exceed {length} character(s)");
        return context;
    }
    
    /// <summary>
    /// Adds a validation action that reports an error if the client provided value's length is greater
    /// than the given string length or is <c>null</c>
    /// </summary>
    /// <param name="context"><see cref="ValidationContext{TModel,TValue}"/></param>
    /// <param name="length">The maximum length of the string.</param>
    /// <param name="messageProvider">A function that provides a custom message when the validation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this provided <see cref="ValidationContext{TModel,TValue}"/> instance.</returns>
    public static ValidationContext<TModel, string?> MaximumLengthOrNull<TModel>(
        this ValidationContext<TModel, string?> context,
        int length,
        Func<string, string>? messageProvider = null) 
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentOutOfRangeException.ThrowIfLessThan(length, 0);
        
        if (context.Value is null || context.Value.Length <= length)
            return context;
        
        context.AddValidationError(messageProvider?.Invoke(context.Value) 
                                ?? $"value must not exceed {length} character(s) or omitted entirely");
        return context;
    }
    
    /// <summary>
    /// Adds a validation action that reports an error if the client provided value is not a match
    /// for the given regular expression.
    /// </summary>
    /// <param name="context"><see cref="ValidationContext{TModel,TValue}"/></param>
    /// <param name="regex">The regex instance..</param>
    /// <param name="messageProvider">A function that provides a custom message when the validation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this provided <see cref="ValidationContext{TModel,TValue}"/> instance.</returns>
    public static ValidationContext<TModel, string> MatchesPattern<TModel>(
        this ValidationContext<TModel, string> context,
        Regex regex,
        Func<string, string>? messageProvider = null) 
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(regex);

        if (regex.IsMatch(context.Value))
            return context;
        
        context.AddValidationError(messageProvider?.Invoke(context.Value) 
                                ?? $"value must match pattern {regex}");
        return context;
    }
    
    /// <summary>
    /// Adds a validation action that reports an error if the client provided value is not null and not a match
    /// for the given regular expression.
    /// </summary>
    /// <param name="context"><see cref="ValidationContext{TModel,TValue}"/></param>
    /// <param name="regex">The regex instance..</param>
    /// <param name="messageProvider">A function that provides a custom message when the validation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this provided <see cref="ValidationContext{TModel,TValue}"/> instance.</returns>
    public static ValidationContext<TModel, string?> MatchesPatternOrNull<TModel>(
        this ValidationContext<TModel, string?> context,
        Regex regex,
        Func<string, string>? messageProvider = null) 
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(regex);

        if (context.Value is null || regex.IsMatch(context.Value))
            return context;
        
        context.AddValidationError(messageProvider?.Invoke(context.Value) 
                                ?? $"value must match pattern {regex} or omitted entirely");
        return context;
    }
}