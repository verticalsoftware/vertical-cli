using System.Text.RegularExpressions;

namespace Vertical.Cli.Validation;

/// <summary>
/// Extensions for string validation.
/// </summary>
public static class StringValidationExtensions
{
    /// <summary>
    /// Adds a rule that the provided string must have a minimum number of characters.
    /// </summary>
    /// <param name="instance">Builder instance</param>
    /// <param name="length">Minimum Length</param>
    /// <param name="message">The message to report if the validation fails</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this instance</returns>
    public static ValidationBuilder<TModel, string> MinimumLength<TModel>(
        this ValidationBuilder<TModel, string> instance,
        int length,
        string? message = null)
        where TModel : class
    {
        return instance.Must((_, value) => value.Length >= length,
            message ?? $"must have at least {length} character(s)");
    }
    
    /// <summary>
    /// Adds a rule that the provided string cannot exceed a maximum number of characters.
    /// </summary>
    /// <param name="instance">Builder instance</param>
    /// <param name="length">Maximum Length</param>
    /// <param name="message">The message to report if the validation fails</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this instance</returns>
    public static ValidationBuilder<TModel, string> MaximumLength<TModel>(
        this ValidationBuilder<TModel, string> instance,
        int length,
        string? message = null)
        where TModel : class
    {
        return instance.Must((_, value) => value.Length <= length,
            message ?? $"cannot exceed {length} character(s)");
    }
    
    /// <summary>
    /// Adds a rule that the provided string must have a specific count of characters.
    /// </summary>
    /// <param name="instance">Builder instance</param>
    /// <param name="length">Length</param>
    /// <param name="message">The message to report if the validation fails</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this instance</returns>
    public static ValidationBuilder<TModel, string> ExactLength<TModel>(
        this ValidationBuilder<TModel, string> instance,
        int length,
        string? message = null)
        where TModel : class
    {
        return instance.Must((_, value) => value.Length == length,
            message ?? $"must have {length} character(s)");
    }

    /// <summary>
    /// Adds a rule that the provided string must start with a specific substring.
    /// </summary>
    /// <param name="instance">Builder instance</param>
    /// <param name="str">Substring the value must start with</param>
    /// <param name="comparison">The string comparison type</param>
    /// <param name="message">The message to report if the validation fails</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this instance</returns>
    public static ValidationBuilder<TModel, string> StartsWith<TModel>(
        this ValidationBuilder<TModel, string> instance,
        string str,
        StringComparison comparison = StringComparison.Ordinal,
        string? message = null)
        where TModel : class
    {
        return instance.Must((_, value) => value.StartsWith(str, comparison),
            message ?? $"must start with '{str}'");
    }
    
    /// <summary>
    /// Adds a rule that the provided string must end with a specific substring.
    /// </summary>
    /// <param name="instance">Builder instance</param>
    /// <param name="str">Substring the value must end with</param>
    /// <param name="comparison">The string comparison type</param>
    /// <param name="message">The message to report if the validation fails</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this instance</returns>
    public static ValidationBuilder<TModel, string> EndsWith<TModel>(
        this ValidationBuilder<TModel, string> instance,
        string str,
        StringComparison comparison = StringComparison.Ordinal,
        string? message = null)
        where TModel : class
    {
        return instance.Must((_, value) => value.EndsWith(str, comparison),
            message ?? $"must end with '{str}'");
    }
    
    /// <summary>
    /// Adds a rule that the provided string must contain a substring.
    /// </summary>
    /// <param name="instance">Builder instance</param>
    /// <param name="str">Substring the value must contain</param>
    /// <param name="comparison">The string comparison type</param>
    /// <param name="message">The message to report if the validation fails</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this instance</returns>
    public static ValidationBuilder<TModel, string> Contains<TModel>(
        this ValidationBuilder<TModel, string> instance,
        string str,
        StringComparison comparison = StringComparison.Ordinal,
        string? message = null)
        where TModel : class
    {
        return instance.Must((_, value) => value.Contains(str, comparison),
            message ?? $"must contain '{str}'");
    }
    
    /// <summary>
    /// Adds a rule that the provided string must match a regular expression.
    /// </summary>
    /// <param name="instance">Builder instance</param>
    /// <param name="pattern">Regular expression pattern</param>
    /// <param name="options">Regular expression options</param>
    /// <param name="message">The message to report if the validation fails</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this instance</returns>
    public static ValidationBuilder<TModel, string> Matches<TModel>(
        this ValidationBuilder<TModel, string> instance,
        string pattern,
        RegexOptions options = RegexOptions.None,
        string? message = null)
        where TModel : class
    {
        return instance.Must((_, value) => Regex.IsMatch(value, pattern, options),
            message ?? $"must match pattern '{pattern}'");
    }
    
    /// <summary>
    /// Adds a rule that the provided string must have a minimum number of characters.
    /// </summary>
    /// <param name="instance">Builder instance</param>
    /// <param name="length">Minimum Length</param>
    /// <param name="message">The message to report if the validation fails</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this instance</returns>
    public static ValidationBuilder<TModel, string?> MinimumLengthOrNull<TModel>(
        this ValidationBuilder<TModel, string?> instance,
        int length,
        string? message = null)
        where TModel : class
    {
        return instance.Must((_, value) => value is null || value.Length >= length,
            message ?? $"must have at least {length} character(s)");
    }
    
    /// <summary>
    /// Adds a rule that the provided string cannot exceed a maximum number of characters.
    /// </summary>
    /// <param name="instance">Builder instance</param>
    /// <param name="length">Maximum Length</param>
    /// <param name="message">The message to report if the validation fails</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this instance</returns>
    public static ValidationBuilder<TModel, string?> MaximumLengthOrNull<TModel>(
        this ValidationBuilder<TModel, string?> instance,
        int length,
        string? message = null)
        where TModel : class
    {
        return instance.Must((_, value) => value is null || value.Length <= length,
            message ?? $"cannot exceed {length} character(s)");
    }
    
    /// <summary>
    /// Adds a rule that the provided string must have a specific count of characters.
    /// </summary>
    /// <param name="instance">Builder instance</param>
    /// <param name="length">Length</param>
    /// <param name="message">The message to report if the validation fails</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this instance</returns>
    public static ValidationBuilder<TModel, string?> ExactLengthOrNull<TModel>(
        this ValidationBuilder<TModel, string?> instance,
        int length,
        string? message = null)
        where TModel : class
    {
        return instance.Must((_, value) => value is null || value.Length == length,
            message ?? $"must have {length} character(s)");
    }

    /// <summary>
    /// Adds a rule that the provided string must start with a specific substring.
    /// </summary>
    /// <param name="instance">Builder instance</param>
    /// <param name="str">Substring the value must start with</param>
    /// <param name="comparison">The string comparison type</param>
    /// <param name="message">The message to report if the validation fails</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this instance</returns>
    public static ValidationBuilder<TModel, string?> StartsWithOrNull<TModel>(
        this ValidationBuilder<TModel, string?> instance,
        string str,
        StringComparison comparison = StringComparison.Ordinal,
        string? message = null)
        where TModel : class
    {
        return instance.Must((_, value) => value is null || value.StartsWith(str, comparison),
            message ?? $"must start with '{str}'");
    }
    
    /// <summary>
    /// Adds a rule that the provided string must end with a specific substring.
    /// </summary>
    /// <param name="instance">Builder instance</param>
    /// <param name="str">Substring the value must end with</param>
    /// <param name="comparison">The string comparison type</param>
    /// <param name="message">The message to report if the validation fails</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this instance</returns>
    public static ValidationBuilder<TModel, string?> EndsWithOrNull<TModel>(
        this ValidationBuilder<TModel, string?> instance,
        string str,
        StringComparison comparison = StringComparison.Ordinal,
        string? message = null)
        where TModel : class
    {
        return instance.Must((_, value) => value is null || value.EndsWith(str, comparison),
            message ?? $"must end with '{str}'");
    }
    
    /// <summary>
    /// Adds a rule that the provided string must contain a substring.
    /// </summary>
    /// <param name="instance">Builder instance</param>
    /// <param name="str">Substring the value must contain</param>
    /// <param name="comparison">The string comparison type</param>
    /// <param name="message">The message to report if the validation fails</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this instance</returns>
    public static ValidationBuilder<TModel, string?> ContainsOrNull<TModel>(
        this ValidationBuilder<TModel, string?> instance,
        string str,
        StringComparison comparison = StringComparison.Ordinal,
        string? message = null)
        where TModel : class
    {
        return instance.Must((_, value) => value is null || value.Contains(str, comparison),
            message ?? $"must contain '{str}'");
    }
    
    /// <summary>
    /// Adds a rule that the provided string must match a regular expression.
    /// </summary>
    /// <param name="instance">Builder instance</param>
    /// <param name="pattern">Regular expression pattern</param>
    /// <param name="options">Regular expression options</param>
    /// <param name="message">The message to report if the validation fails</param>
    /// <typeparam name="TModel">Model type</typeparam>
    /// <returns>A reference to this instance</returns>
    public static ValidationBuilder<TModel, string?> MatchesOrNull<TModel>(
        this ValidationBuilder<TModel, string?> instance,
        string pattern,
        RegexOptions options = RegexOptions.None,
        string? message = null)
        where TModel : class
    {
        return instance.Must((_, value) => value is null || Regex.IsMatch(value, pattern, options),
            message ?? $"must match pattern '{pattern}'");
    }
}