using Vertical.Cli.Configuration;

namespace Vertical.Cli.Validation;

/// <summary>
/// Represents a context by which models are validated.
/// </summary>
public sealed class ValidationContext
{
    private readonly List<ValidationError> _errors = new();
    
    internal ValidationContext()
    {
    }

    /// <summary>
    /// Gets validation errors.
    /// </summary>
    public IReadOnlyCollection<ValidationError> Errors => _errors;

    /// <summary>
    /// Gets whether all validations passed.
    /// </summary>
    public bool IsValid => _errors.Count == 0;
    
    /// <summary>
    /// Adds an error.
    /// </summary>
    /// <param name="symbol">Mapped symbol</param>
    /// <param name="attemptedValue">Value that was attempted</param>
    /// <param name="message">Description of the error</param>
    public void AddError(
        CliSymbol symbol,
        object? attemptedValue,
        string? message)
    {
        _errors.Add(new ValidationError(symbol, attemptedValue, message));    
    }
}