using System.Text.RegularExpressions;
using CommunityToolkit.Diagnostics;

namespace Vertical.Cli.Validation;

/// <summary>
/// Extensions
/// </summary>
public static class ValidationBuilderExtensions
{
    /// <summary>
    /// Adds an evaluator in the form of a predicate.
    /// </summary>
    /// <param name="obj">ValidationBuilder</param>
    /// <param name="evaluator">Function that receives the model and candidate value and returns
    /// whether the value is valid.</param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static ValidationBuilder<TModel, TValue> Must<TModel, TValue>(
        this ValidationBuilder<TModel, TValue> obj,
        Func<TModel, TValue, bool> evaluator,
        string? message = null)
    {
        Guard.IsNotNull(evaluator);
        
        return obj.Use((model, value) => evaluator(model, value)
            ? ValidationResult.Ok
            : ValidationResult.Invalid(value, message));
    }
    
    /// <summary>
    /// Adds an evaluator that checks if the candidate value is less than the specified value.
    /// </summary>
    /// <param name="obj">ValidationBuilder</param>
    /// <param name="value">Value that represents the acceptable condition.</param>
    /// <param name="message">Message to provide if the evaluation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <returns>A reference to this instance.</returns>
    public static ValidationBuilder<TModel, TValue> LessThan<TModel, TValue>(
        this ValidationBuilder<TModel, TValue> obj,
        TValue value,
        string? message = null)
        where TValue : IComparable<TValue>
    {
        return obj.Must((_, comparable) => comparable.CompareTo(value) < 0, 
            message ?? $"Value must be less than {value}.");
    }
    
    /// <summary>
    /// Adds an evaluator that checks if the candidate value is less than the specified value.
    /// </summary>
    /// <param name="obj">ValidationBuilder</param>
    /// <param name="value">Value that represents the acceptable condition.</param>
    /// <param name="message">Message to provide if the evaluation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <returns>A reference to this instance.</returns>
    public static ValidationBuilder<TModel, TValue?> LessThan<TModel, TValue>(
        this ValidationBuilder<TModel, TValue?> obj,
        TValue value,
        string? message = null)
        where TValue : struct, IComparable<TValue>
    {
        return obj.Must((_, comparable) => !comparable.HasValue || comparable.Value.CompareTo(value) < 0, 
            message ?? $"Value must be less than {value}.");
    }
    
    /// <summary>
    /// Adds an evaluator that checks if the candidate value is less than or equal to the specified value.
    /// </summary>
    /// <param name="obj">ValidationBuilder</param>
    /// <param name="value">Value that represents the acceptable condition.</param>
    /// <param name="message">Message to provide if the evaluation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <returns>A reference to this instance.</returns>
    public static ValidationBuilder<TModel, TValue> LessThanOrEqual<TModel, TValue>(
        this ValidationBuilder<TModel, TValue> obj,
        TValue value,
        string? message = null)
        where TValue : IComparable<TValue>
    {
        return obj.Must((_, comparable) => comparable.CompareTo(value) <= 0, 
            message ?? $"Value must be less than or equal {value}.");
    }
    
    /// <summary>
    /// Adds an evaluator that checks if the candidate value is less than or equal to the specified value.
    /// </summary>
    /// <param name="obj">ValidationBuilder</param>
    /// <param name="value">Value that represents the acceptable condition.</param>
    /// <param name="message">Message to provide if the evaluation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <returns>A reference to this instance.</returns>
    public static ValidationBuilder<TModel, TValue?> LessThanOrEqual<TModel, TValue>(
        this ValidationBuilder<TModel, TValue?> obj,
        TValue value,
        string? message = null)
        where TValue : struct, IComparable<TValue>
    {
        return obj.Must((_, comparable) => !comparable.HasValue || comparable.Value.CompareTo(value) <= 0, 
            message ?? $"Value must be less than or equal {value}.");
    }
    
    /// <summary>
    /// Adds an evaluator that checks if the candidate value is greater than the specified value.
    /// </summary>
    /// <param name="obj">ValidationBuilder</param>
    /// <param name="value">Value that represents the acceptable condition.</param>
    /// <param name="message">Message to provide if the evaluation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <returns>A reference to this instance.</returns>
    public static ValidationBuilder<TModel, TValue> GreaterThan<TModel, TValue>(
        this ValidationBuilder<TModel, TValue> obj,
        TValue value,
        string? message = null)
        where TValue : IComparable<TValue>
    {
        return obj.Must((_, comparable) => comparable.CompareTo(value) > 0, 
            message ?? $"Value must be greater than {value}.");
    }
    
    /// <summary>
    /// Adds an evaluator that checks if the candidate value is greater than the specified value.
    /// </summary>
    /// <param name="obj">ValidationBuilder</param>
    /// <param name="value">Value that represents the acceptable condition.</param>
    /// <param name="message">Message to provide if the evaluation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <returns>A reference to this instance.</returns>
    public static ValidationBuilder<TModel, TValue?> GreaterThan<TModel, TValue>(
        this ValidationBuilder<TModel, TValue?> obj,
        TValue value,
        string? message = null)
        where TValue : struct, IComparable<TValue>
    {
        return obj.Must((_, comparable) => !comparable.HasValue || comparable.Value.CompareTo(value) > 0, 
            message ?? $"Value must be greater than {value}.");
    }
    
    /// <summary>
    /// Adds an evaluator that checks if the candidate value is greater than or equal to the specified value.
    /// </summary>
    /// <param name="obj">ValidationBuilder</param>
    /// <param name="value">Value that represents the acceptable condition.</param>
    /// <param name="message">Message to provide if the evaluation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <returns>A reference to this instance.</returns>
    public static ValidationBuilder<TModel, TValue> GreaterThanOrEqual<TModel, TValue>(
        this ValidationBuilder<TModel, TValue> obj,
        TValue value,
        string? message = null)
        where TValue : IComparable<TValue>
    {
        return obj.Must((_, comparable) => comparable.CompareTo(value) >= 0, 
            message ?? $"Value must be greater than or equal {value}.");
    }
    
    /// <summary>
    /// Adds an evaluator that checks if the candidate value is greater than or equal to the specified value.
    /// </summary>
    /// <param name="obj">ValidationBuilder</param>
    /// <param name="value">Value that represents the acceptable condition.</param>
    /// <param name="message">Message to provide if the evaluation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <returns>A reference to this instance.</returns>
    public static ValidationBuilder<TModel, TValue?> GreaterThanOrEqual<TModel, TValue>(
        this ValidationBuilder<TModel, TValue?> obj,
        TValue value,
        string? message = null)
        where TValue : struct, IComparable<TValue>
    {
        return obj.Must((_, comparable) => !comparable.HasValue || comparable.Value.CompareTo(value) >= 0, 
            message ?? $"Value must be greater than or equal {value}.");
    }
    
    /// <summary>
    /// Adds an evaluator that checks if a file exists.
    /// </summary>
    /// <param name="obj">ValidationBuilder</param>
    /// <param name="message">Message to provide if the evaluation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this instance.</returns>
    public static ValidationBuilder<TModel, FileInfo> Exists<TModel>(
        this ValidationBuilder<TModel, FileInfo> obj,
        string? message = null)
    {
        return obj.Must((_, fi) => fi.Exists, message ?? "File not found.");
    }
    
    /// <summary>
    /// Adds an evaluator that checks if a file exists.
    /// </summary>
    /// <param name="obj">ValidationBuilder</param>
    /// <param name="message">Message to provide if the evaluation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this instance.</returns>
    public static ValidationBuilder<TModel, FileInfo?> ExistsOrIsNull<TModel>(
        this ValidationBuilder<TModel, FileInfo?> obj,
        string? message = null)
    {
        return obj.Must((_, fi) => fi is null || fi.Exists, message ?? "File not found.");
    }
    
    /// <summary>
    /// Adds an evaluator that checks if a file does not exist.
    /// </summary>
    /// <param name="obj">ValidationBuilder</param>
    /// <param name="message">Message to provide if the evaluation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this instance.</returns>
    public static ValidationBuilder<TModel, FileInfo> DoesNotExist<TModel>(
        this ValidationBuilder<TModel, FileInfo> obj,
        string? message = null)
    {
        return obj.Must((_, fi) => !fi.Exists, message ?? "File already exists.");
    }
    
    /// <summary>
    /// Adds an evaluator that checks if a file does not exist.
    /// </summary>
    /// <param name="obj">ValidationBuilder</param>
    /// <param name="message">Message to provide if the evaluation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this instance.</returns>
    public static ValidationBuilder<TModel, FileInfo?> DoesNotExistOrIsNull<TModel>(
        this ValidationBuilder<TModel, FileInfo?> obj,
        string? message = null)
    {
        return obj.Must((_, fi) => fi is null || !fi.Exists, message ?? "File already exists.");
    }
    
    /// <summary>
    /// Adds an evaluator that checks if a directory exists.
    /// </summary>
    /// <param name="obj">ValidationBuilder</param>
    /// <param name="message">Message to provide if the evaluation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this instance.</returns>
    public static ValidationBuilder<TModel, DirectoryInfo> Exists<TModel>(
        this ValidationBuilder<TModel, DirectoryInfo> obj,
        string? message = null)
    {
        return obj.Must((_, di) => di.Exists, message ?? "Directory not found.");
    }
    
    /// <summary>
    /// Adds an evaluator that checks if a directory exists.
    /// </summary>
    /// <param name="obj">ValidationBuilder</param>
    /// <param name="message">Message to provide if the evaluation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this instance.</returns>
    public static ValidationBuilder<TModel, DirectoryInfo?> ExistsOrIsNull<TModel>(
        this ValidationBuilder<TModel, DirectoryInfo?> obj,
        string? message = null)
    {
        return obj.Must((_, di) => di is null || di.Exists, 
            message ?? "Directory not found.");
    }
    
    /// <summary>
    /// Adds an evaluator that checks if a directory does not exist.
    /// </summary>
    /// <param name="obj">ValidationBuilder</param>
    /// <param name="message">Message to provide if the evaluation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this instance.</returns>
    public static ValidationBuilder<TModel, DirectoryInfo> DoesNotExist<TModel>(
        this ValidationBuilder<TModel, DirectoryInfo> obj,
        string? message = null)
    {
        return obj.Must((_, di) => !di.Exists, message ?? "Directory already exists.");
    }
    
    /// <summary>
    /// Adds an evaluator that checks if a directory does not exist.
    /// </summary>
    /// <param name="obj">ValidationBuilder</param>
    /// <param name="message">Message to provide if the evaluation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this instance.</returns>
    public static ValidationBuilder<TModel, DirectoryInfo?> DoesNotExistOrIsNull<TModel>(
        this ValidationBuilder<TModel, DirectoryInfo?> obj,
        string? message = null)
    {
        return obj.Must((_, di) => di is null || !di.Exists, 
            message ?? "Directory already exists.");
    }
    
    /// <summary>
    /// Adds an evaluator that checks if the candidate value is one of the provided values.
    /// </summary>
    /// <param name="obj">ValidationBuilder</param>
    /// <param name="values">Values that the candidate value must match.</param>
    /// <param name="comparer">Comparison implementation to use.</param>
    /// <param name="message">Message to provide if the evaluation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <returns>A reference to this instance.</returns>
    public static ValidationBuilder<TModel, TValue> In<TModel, TValue>(
        this ValidationBuilder<TModel, TValue> obj,
        IEnumerable<TValue> values,
        IEqualityComparer<TValue>? comparer = null,
        string? message = null)
    {
        comparer ??= EqualityComparer<TValue>.Default;
        
        return obj.Must((_, value) => values.Any(item => comparer.Equals(value, item)), 
            message);
    }
    
    /// <summary>
    /// Adds an evaluator that checks if the candidate value is one of the provided values.
    /// </summary>
    /// <param name="obj">ValidationBuilder</param>
    /// <param name="values">Values that the candidate value must match.</param>
    /// <param name="comparer">Comparison implementation to use.</param>
    /// <param name="message">Message to provide if the evaluation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <returns>A reference to this instance.</returns>
    public static ValidationBuilder<TModel, TValue?> InOrIsNull<TModel, TValue>(
        this ValidationBuilder<TModel, TValue?> obj,
        IEnumerable<TValue> values,
        IEqualityComparer<TValue>? comparer = null,
        string? message = null)
    {
        comparer ??= EqualityComparer<TValue>.Default;
        
        return obj.Must((_, value) => value is null || values.Any(item => comparer.Equals(value, item)), 
            message);
    }
    
    /// <summary>
    /// Adds an evaluator that checks if the candidate value is not one of the provided values.
    /// </summary>
    /// <param name="obj">ValidationBuilder</param>
    /// <param name="values">Values that the candidate value must not match.</param>
    /// <param name="comparer">Comparison implementation to use.</param>
    /// <param name="message">Message to provide if the evaluation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <returns>A reference to this instance.</returns>
    public static ValidationBuilder<TModel, TValue> NotIn<TModel, TValue>(
        this ValidationBuilder<TModel, TValue> obj,
        IEnumerable<TValue> values,
        IEqualityComparer<TValue>? comparer = null,
        string? message = null)
    {
        comparer ??= EqualityComparer<TValue>.Default;
        
        return obj.Must((_, value) => values.All(item => !comparer.Equals(value, item)), 
            message);
    }
    
    /// <summary>
    /// Adds an evaluator that checks if the candidate value is not one of the provided values.
    /// </summary>
    /// <param name="obj">ValidationBuilder</param>
    /// <param name="values">Values that the candidate value must not match.</param>
    /// <param name="comparer">Comparison implementation to use.</param>
    /// <param name="message">Message to provide if the evaluation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <returns>A reference to this instance.</returns>
    public static ValidationBuilder<TModel, TValue?> NotInOrIsNull<TModel, TValue>(
        this ValidationBuilder<TModel, TValue?> obj,
        IEnumerable<TValue> values,
        IEqualityComparer<TValue>? comparer = null,
        string? message = null)
    {
        comparer ??= EqualityComparer<TValue>.Default;
        
        return obj.Must((_, value) => values.All(item => value is null || !comparer.Equals(value, item)), 
            message);
    }
    
    /// <summary>
    /// Adds an evaluator that checks if the candidate value meets a minimum length.
    /// </summary>
    /// <param name="obj">ValidationBuilder</param>
    /// <param name="length">Length the string must be.</param>
    /// <param name="message">Message to provide if the evaluation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this instance.</returns>
    public static ValidationBuilder<TModel, string> HasMinLength<TModel>(
        this ValidationBuilder<TModel, string> obj,
        int length,
        string? message = null)
    {
        return obj.Must((_, str) => str.Length >= length, 
            message ?? $"Length must be at least {length} character(s).");
    }
    
    /// <summary>
    /// Adds an evaluator that checks if the candidate value meets a minimum length.
    /// </summary>
    /// <param name="obj">ValidationBuilder</param>
    /// <param name="length">Length the string must be.</param>
    /// <param name="message">Message to provide if the evaluation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this instance.</returns>
    public static ValidationBuilder<TModel, string?> HasMinLengthOrIsNull<TModel>(
        this ValidationBuilder<TModel, string?> obj,
        int length,
        string? message = null)
    {
        return obj.Must((_, str) => str is null || str.Length >= length, 
            message ?? $"Length must be at least {length} character(s).");
    }
    
    /// <summary>
    /// Adds an evaluator that checks if the candidate value exceeds a maximum length.
    /// </summary>
    /// <param name="obj">ValidationBuilder</param>
    /// <param name="length">Length the string cannot exceed.</param>
    /// <param name="message">Message to provide if the evaluation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this instance.</returns>
    public static ValidationBuilder<TModel, string> HasMaxLength<TModel>(
        this ValidationBuilder<TModel, string> obj,
        int length,
        string? message = null)
    {
        return obj.Must((_, str) => str.Length <= length, 
            message ?? $"Length cannot exceed {length} character(s).");
    }
    
    /// <summary>
    /// Adds an evaluator that checks if the candidate value exceeds a maximum length.
    /// </summary>
    /// <param name="obj">ValidationBuilder</param>
    /// <param name="length">Length the string cannot exceed.</param>
    /// <param name="message">Message to provide if the evaluation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this instance.</returns>
    public static ValidationBuilder<TModel, string?> HasMaxLengthOrIsNull<TModel>(
        this ValidationBuilder<TModel, string?> obj,
        int length,
        string? message = null)
    {
        return obj.Must((_, str) => str is null || str.Length <= length, 
            message ?? $"Length cannot exceed {length} character(s).");
    }
    
    /// <summary>
    /// Adds an evaluator that checks if the candidate value matches a pattern.
    /// </summary>
    /// <param name="obj">ValidationBuilder</param>
    /// <param name="pattern">Pattern the string must match.</param>
    /// <param name="message">Message to provide if the evaluation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this instance.</returns>
    public static ValidationBuilder<TModel, string> Matches<TModel>(
        this ValidationBuilder<TModel, string> obj,
        string pattern,
        string? message = null)
    {
        return obj.Must((_, str) => Regex.IsMatch(str, pattern), 
            message ?? "Value does not match the required pattern.");
    }
    
    /// <summary>
    /// Adds an evaluator that checks if the candidate value matches a pattern.
    /// </summary>
    /// <param name="obj">ValidationBuilder</param>
    /// <param name="pattern">Pattern the string must match.</param>
    /// <param name="message">Message to provide if the evaluation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this instance.</returns>
    public static ValidationBuilder<TModel, string?> MatchesOrIsNull<TModel>(
        this ValidationBuilder<TModel, string?> obj,
        string pattern,
        string? message = null)
    {
        return obj.Must((_, str) => str is null || Regex.IsMatch(str, pattern), 
            message ?? "Value does not match the required pattern.");
    }
    
    /// <summary>
    /// Adds an evaluator that checks if the candidate value does not match a pattern.
    /// </summary>
    /// <param name="obj">ValidationBuilder</param>
    /// <param name="pattern">Pattern the string must not match.</param>
    /// <param name="message">Message to provide if the evaluation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this instance.</returns>
    public static ValidationBuilder<TModel, string> DoesNotMatch<TModel>(
        this ValidationBuilder<TModel, string> obj,
        string pattern,
        string? message = null)
    {
        return obj.Must((_, str) => !Regex.IsMatch(str, pattern), 
            message ?? "Value matches an invalid pattern.");
    }
    
    /// <summary>
    /// Adds an evaluator that checks if the candidate value does not match a pattern.
    /// </summary>
    /// <param name="obj">ValidationBuilder</param>
    /// <param name="pattern">Pattern the string must not match.</param>
    /// <param name="message">Message to provide if the evaluation fails.</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this instance.</returns>
    public static ValidationBuilder<TModel, string?> DoesNotMatchOrIsNull<TModel>(
        this ValidationBuilder<TModel, string?> obj,
        string pattern,
        string? message = null)
    {
        return obj.Must((_, str) => str is null || !Regex.IsMatch(str, pattern), 
            message ?? "Value matches an invalid pattern.");
    }
}