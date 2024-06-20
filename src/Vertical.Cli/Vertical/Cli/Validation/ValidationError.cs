using Vertical.Cli.Configuration;

namespace Vertical.Cli.Validation
{
    /// <summary>
    /// Defines a validation error.
    /// </summary>
    /// <param name="Symbol">The symbol associated with the error.</param>
    /// <param name="AttemptedValue">The attempted value.</param>
    /// <param name="Error">The error message that describes the condition.</param>
    public record ValidationError(CliSymbol Symbol, object? AttemptedValue, string? Error);
}