using System.Text.RegularExpressions;
using CommunityToolkit.Diagnostics;

namespace Vertical.Cli.Validation;

/// <summary>
/// Defines methods used to build validation rules.
/// </summary>
public static class ValidationBuilderExtensions
{
    /// <summary>
    /// Adds a validator that determines if a string meets a minimum length.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="length">The minimum length the candidate string must be.</param>
    /// <param name="message">A function that returns the message to display if validation fails.</param>
    /// <returns>A reference to the provided instance.</returns>
    public static ValidationBuilder<string> MinimumLength(
        this ValidationBuilder<string> builder, 
        int length,
        Func<string>? message = null)
    {
        Guard.IsNotNull(builder);
        Guard.IsGreaterThan(length, 0);

        return builder.Must(
            value => value.Length >= length, 
            message ?? (() => $"Length must be {length} or more characters."));
    }
    
    /// <summary>
    /// Adds a validator that determines if a string does not exceed a maximum length.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="length">The maximum length the candidate string can be.</param>
    /// <param name="message">A function that returns the message to display if validation fails.</param>
    /// <returns>A reference to the provided instance.</returns>
    public static ValidationBuilder<string> MaximumLength(
        this ValidationBuilder<string> builder, 
        int length,
        Func<string>? message = null)
    {
        Guard.IsNotNull(builder);
        Guard.IsGreaterThan(length, 0);

        return builder.Must(
            value => value.Length <= length,
            message ?? (() => $"Length must be {length} or less characters."));
    }
    
    /// <summary>
    /// Adds a validator that determines if a string meets an exact length.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="length">The length the candidate string must be.</param>
    /// <param name="message">A function that returns the message to display if validation fails.</param>
    /// <returns>A reference to the provided instance.</returns>
    public static ValidationBuilder<string> ExactLength(
        this ValidationBuilder<string> builder, 
        int length,
        Func<string>? message = null)
    {
        Guard.IsNotNull(builder);
        Guard.IsGreaterThan(length, 0);

        return builder.Must(
            value => value.Length == length, 
            message ?? (() => $"Length must be {length} characters."));
    }

    /// <summary>
    /// Adds a validator that determines if the length of a string value is within a range.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="minimumLength">The minimum length the candidate string must be.</param>
    /// <param name="maximumLength">The maximum length the candidate string can be.</param>
    /// <param name="message">A function that returns the message to display if validation fails.</param>
    /// <returns>A reference to the provided instance.</returns>
    public static ValidationBuilder<string> LengthBetween(this ValidationBuilder<string> builder, 
        int minimumLength,
        int maximumLength,
        Func<string>? message = null)
    {
        Guard.IsNotNull(builder);
        Guard.IsGreaterThan(minimumLength, 0);
        Guard.IsLessThanOrEqualTo(maximumLength, minimumLength);

        return builder.Must(
            value => value.Length >= minimumLength && value.Length <= maximumLength, 
            message ?? (() => $"Length must be between {minimumLength} and {maximumLength} characters."));
    }

    /// <summary>
    /// Adds a validator that determines if a value starts with specified substring.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="subString">The substring to match.</param>
    /// <param name="comparisonType">The string comparison used in the search.</param>
    /// <param name="message">A function that returns the message to display if validation fails.</param>
    /// <returns>A reference to the provided instance.</returns>
    public static ValidationBuilder<string> StartsWith(this ValidationBuilder<string> builder, 
        string subString,
        StringComparison comparisonType = StringComparison.Ordinal,
        Func<string>? message = null)
    {
        Guard.IsNotNull(builder);
        Guard.IsNotNull(subString);

        return builder.Must(value => value.StartsWith(subString, comparisonType),
            message ?? (() => $"Value must start with \"{subString}\"."));
    }
    
    /// <summary>
    /// Adds a validator that determines if a value ends with specified substring.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="subString">The substring to match.</param>
    /// <param name="comparisonType">The string comparison used in the search.</param>
    /// <param name="message">A function that returns the message to display if validation fails.</param>
    /// <returns>A reference to the provided instance.</returns>
    public static ValidationBuilder<string> EndsWith(this ValidationBuilder<string> builder, 
        string subString,
        StringComparison comparisonType = StringComparison.Ordinal,
        Func<string>? message = null)
    {
        Guard.IsNotNull(builder);
        Guard.IsNotNull(subString);

        return builder.Must(value => value.EndsWith(subString, comparisonType),
            message ?? (() => $"Value must start with \"{subString}\"."));
    }
    
    /// <summary>
    /// Adds a validator that determines if a value contains the specified substring.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="subString">The substring to match.</param>
    /// <param name="comparisonType">The string comparison used in the search.</param>
    /// <param name="message">A function that returns the message to display if validation fails.</param>
    /// <returns>A reference to the provided instance.</returns>
    public static ValidationBuilder<string> Contains(this ValidationBuilder<string> builder, 
        string subString,
        StringComparison comparisonType = StringComparison.Ordinal,
        Func<string>? message = null)
    {
        Guard.IsNotNull(builder);
        Guard.IsNotNull(subString);

        return builder.Must(value => value.Contains(subString, comparisonType),
            message ?? (() => $"Value must contain substring \"{subString}\"."));
    }
    
    /// <summary>
    /// Adds a validator that determines if a value matches a regular expression pattern.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="pattern">The regular expression pattern.</param>
    /// <param name="message">A function that returns the message to display if validation fails.</param>
    /// <returns>A reference to the provided instance.</returns>
    public static ValidationBuilder<string> Matches(
        this ValidationBuilder<string> builder, 
        string pattern,
        Func<string>? message = null)
    {
        Guard.IsNotNull(builder);
        Guard.IsNotNull(pattern);

        return builder.Matches(new Regex(pattern), message);
    }

    /// <summary>
    /// Adds a validator that determines if a value matches a regular expression pattern.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="regex">The regular expression instance.</param>
    /// <param name="message">A function that returns the message to display if validation fails.</param>
    /// <returns>A reference to the provided instance.</returns>
    public static ValidationBuilder<string> Matches(
        this ValidationBuilder<string> builder, 
        Regex regex,
        Func<string>? message = null)
    {
        Guard.IsNotNull(builder);
        Guard.IsNotNull(regex);

        return builder.Must(
            regex.IsMatch, 
            message ?? (() => $"Value must match pattern {regex}."));
    }

    /// <summary>
    /// Adds a validator that determines if a value is less than a specified value.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="value">The value to compare.</param>
    /// <param name="comparer">The comparer implementation, or <c>null</c> to use the default comparer.</param>
    /// <param name="message">A function that returns the message to display if validation fails.</param>
    /// <typeparam name="T">The value type.</typeparam>
    /// <returns>A reference to the provided instance.</returns>
    public static ValidationBuilder<T> LessThan<T>(
        this ValidationBuilder<T> builder,
        T value,
        IComparer<T>? comparer = null,
        Func<string>? message = null)
        where T : IComparable<T>
    {
        Guard.IsNotNull(builder);
        comparer ??= Comparer<T>.Default;

        return builder.Must(
            comparable => comparer.Compare(value, comparable) < 0,
            message ?? (() => $"Value must be less than {new DisplayValue<T>(value)}."));
    }
    
    /// <summary>
    /// Adds a validator that determines if a value is less than or equal to a specified value.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="value">The value to compare.</param>
    /// <param name="comparer">The comparer implementation, or <c>null</c> to use the default comparer.</param>
    /// <param name="message">A function that returns the message to display if validation fails.</param>
    /// <typeparam name="T">The value type.</typeparam>
    /// <returns>A reference to the provided instance.</returns>
    public static ValidationBuilder<T> LessThanOrEquals<T>(
        this ValidationBuilder<T> builder,
        T value,
        IComparer<T>? comparer = null,
        Func<string>? message = null)
        where T : IComparable<T>
    {
        Guard.IsNotNull(builder);
        comparer ??= Comparer<T>.Default;

        return builder.Must(
            comparable => comparer.Compare(value, comparable) <= 0,
            message ?? (() => $"Value must be less than or equal to {new DisplayValue<T>(value)}."));
    }
    
    /// <summary>
    /// Adds a validator that determines if a value is greater than a specified value.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="value">The value to compare.</param>
    /// <param name="comparer">The comparer implementation, or <c>null</c> to use the default comparer.</param>
    /// <param name="message">A function that returns the message to display if validation fails.</param>
    /// <typeparam name="T">The value type.</typeparam>
    /// <returns>A reference to the provided instance.</returns>
    public static ValidationBuilder<T> GreaterThan<T>(
        this ValidationBuilder<T> builder,
        T value,
        IComparer<T>? comparer = null,
        Func<string>? message = null)
        where T : IComparable<T>
    {
        Guard.IsNotNull(builder);
        comparer ??= Comparer<T>.Default;

        return builder.Must(
            comparable => comparer.Compare(value, comparable) > 0,
            message ?? (() => $"Value must be greater than {new DisplayValue<T>(value)}."));
    }
    
    /// <summary>
    /// Adds a validator that determines if a value is equal to or greater than a specified value.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="value">The value to compare.</param>
    /// <param name="comparer">The comparer implementation, or <c>null</c> to use the default comparer.</param>
    /// <param name="message">A function that returns the message to display if validation fails.</param>
    /// <typeparam name="T">The value type.</typeparam>
    /// <returns>A reference to the provided instance.</returns>
    public static ValidationBuilder<T> GreaterThanOrEquals<T>(
        this ValidationBuilder<T> builder,
        T value,
        IComparer<T>? comparer = null,
        Func<string>? message = null)
        where T : IComparable<T>
    {
        Guard.IsNotNull(builder);
        comparer ??= Comparer<T>.Default;

        return builder.Must(
            comparable => comparer.Compare(value, comparable) >= 0,
            message ?? (() => $"Value must be greater than {new DisplayValue<T>(value)}."));
    }
    
    /// <summary>
    /// Adds a validator that determines if a value is not the specified value.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="value">The value to compare.</param>
    /// <param name="comparer">The comparer implementation, or <c>null</c> to use the default comparer.</param>
    /// <param name="message">A function that returns the message to display if validation fails.</param>
    /// <typeparam name="T">The value type.</typeparam>
    /// <returns>A reference to the provided instance.</returns>
    public static ValidationBuilder<T> Not<T>(
        this ValidationBuilder<T> builder,
        T value,
        IEqualityComparer<T>? comparer = null,
        Func<string>? message = null)
        where T : IComparable<T>
    {
        Guard.IsNotNull(builder);
        comparer ??= EqualityComparer<T>.Default;

        return builder.Must(
            comparable => !comparer.Equals(value, comparable),
            message ?? (() => $"Value must not be {new DisplayValue<T>(value)}."));
    }

    /// <summary>
    /// Adds a validator that ensures a value matches an element in the provided set.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="values">The set of values.</param>
    /// <param name="comparer">The comparer implementation, or <c>null</c> to use the default comparer.</param>
    /// <param name="message">A function that returns the message to display if validation fails.</param>
    /// <typeparam name="T">The value type.</typeparam>
    /// <returns>A reference to the provided instance.</returns>
    public static ValidationBuilder<T> In<T>(
        this ValidationBuilder<T> builder,
        IEnumerable<T> values,
        IEqualityComparer<T>? comparer = null,
        Func<string>? message = null)
    {
        Guard.IsNotNull(builder);
        comparer ??= EqualityComparer<T>.Default;

        var hashSet = new HashSet<T>(values, comparer);

        return builder.Must(
            comparable => hashSet.Contains(comparable),
            message ?? 
            (() =>
            {
                var csv = string.Join(",", hashSet.Select(v => new DisplayValue<T>(v)));
                return $"Value must be one of [{csv}].";
            }));
    }
    
    /// <summary>
    /// Adds a validator that ensures a value does not match an element in the provided set.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="values">The set of values.</param>
    /// <param name="comparer">The comparer implementation, or <c>null</c> to use the default comparer.</param>
    /// <param name="message">A function that returns the message to display if validation fails.</param>
    /// <typeparam name="T">The value type.</typeparam>
    /// <returns>A reference to the provided instance.</returns>
    public static ValidationBuilder<T> NotIn<T>(
        this ValidationBuilder<T> builder,
        IEnumerable<T> values,
        IEqualityComparer<T>? comparer = null,
        Func<string>? message = null)
    {
        Guard.IsNotNull(builder);
        comparer ??= EqualityComparer<T>.Default;

        var hashSet = new HashSet<T>(values, comparer);

        return builder.Must(
            comparable => !hashSet.Contains(comparable),
            message ??
            (() =>
            {
                var csv = string.Join(",", hashSet.Select(v => new DisplayValue<T>(v)));
                return $"Value must not be one of [{csv}].";
            }));
    }

    /// <summary>
    /// Adds a validator that ensures a <see cref="FileInfo"/> value reference a valid path.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="message">A function that returns the message to display if validation fails.</param>
    /// <returns>A reference to the provided instance.</returns>
    public static ValidationBuilder<FileInfo> FileExists(
        this ValidationBuilder<FileInfo> builder,
        Func<string>? message = null)
    {
        Guard.IsNotNull(builder);

        return builder.Must(
            fileInfo => fileInfo.Exists,
            message ?? (() => "File not found."));
    }
    
    /// <summary>
    /// Adds a validator that ensures a <see cref="FileInfo"/> value reference a valid path when the
    /// argument is not null.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="message">A function that returns the message to display if validation fails.</param>
    /// <returns>A reference to the provided instance.</returns>
    public static ValidationBuilder<FileInfo?> FileExistsIfNotNull(
        this ValidationBuilder<FileInfo?> builder,
        Func<string>? message = null)
    {
        Guard.IsNotNull(builder);

        return builder.Must(
            fileInfo => fileInfo == null || fileInfo.Exists,
            message ?? (() => "File not found."));
    }
    
    /// <summary>
    /// Adds a validator that ensures a <see cref="DirectoryInfo"/> value reference a valid path.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="message">A function that returns the message to display if validation fails.</param>
    /// <returns>A reference to the provided instance.</returns>
    public static ValidationBuilder<DirectoryInfo> DirectoryExists(
        this ValidationBuilder<DirectoryInfo> builder,
        Func<string>? message = null)
    {
        Guard.IsNotNull(builder);

        return builder.Must(
            directoryInfo => directoryInfo.Exists,
            message ?? (() => "Directory not found."));
    }

    /// <summary>
    /// Adds a validator that ensures a <see cref="FileInfo"/> value path reference exists when the argument
    /// is not null.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="message">A function that returns the message to display if validation fails.</param>
    /// <returns>A reference to the provided instance.</returns>
    public static ValidationBuilder<DirectoryInfo?> DirectoryExistsIfNotNull(
        this ValidationBuilder<DirectoryInfo?> builder,
        Func<string>? message = null)
    {
        Guard.IsNotNull(builder);

        return builder.Must(
            directoryInfo => directoryInfo == null || directoryInfo.Exists,
            message ?? (() => "Directory not found."));
    }
}