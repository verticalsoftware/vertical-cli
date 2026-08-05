namespace Vertical.Cli.Validation;

/// <summary>
/// Represents an object that encapsulates its own validation logic.
/// </summary>
public interface IValidatable
{
    /// <summary>
    /// Evaluates its value and reports errors to the given context.
    /// </summary>
    /// <param name="context">The validation context.</param>
    void Validate(ValidationContext context);
}