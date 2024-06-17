namespace Vertical.Cli.Validation;

/// <summary>
/// Describes the result of validation.
/// </summary>
public sealed class ValidationResult
{
    private ValidationResult(bool isValid, object? attemptedValue, string? error)
    {
        IsValid = isValid;
        AttemptedValue = attemptedValue;
        Error = error;
    }

    /// <summary>
    /// Represents a condition in which the validation passed.
    /// </summary>
    public static ValidationResult Ok { get; } = new(true, null,null);

    /// <summary>
    /// Creates a <see cref="ValidationResult"/> that represents a failed state.
    /// </summary>
    /// <param name="attemptedValue">Value that was considered invalid.</param>
    /// <param name="error">Optional error message.</param>
    /// <returns><see cref="ValidationResult"/></returns>
    public static ValidationResult Invalid(
        object? attemptedValue, 
        string? error = null) =>
        new(false, attemptedValue, error);

    /// <summary>
    /// Gets whether validation succeeded.
    /// </summary>
    public bool IsValid { get; }

    /// <summary>
    /// Gets the attempted value.
    /// </summary>
    public object? AttemptedValue { get; }

    /// <summary>
    /// Gets the message associated with the invalid value.
    /// </summary>
    public string? Error { get; }
}