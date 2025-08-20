using Vertical.Cli.Binding;
using Vertical.Cli.Invocation;

namespace Vertical.Cli.Conversion;

/// <summary>
/// Represents an error that occurred when a conversion attempt failed.
/// </summary>
public sealed class ConversionError : UsageError
{
    internal ConversionError(IPropertyBinding binding,
        string attemptedValue,
        Exception? exception)
    {
        Binding = binding;
        AttemptedValue = attemptedValue;
        Exception = exception;
    }

    /// <summary>
    /// Gets the subject being bound when the conversion failed.
    /// </summary>
    public IPropertyBinding Binding { get; }

    /// <summary>
    /// Gets the attempted value.
    /// </summary>
    public string AttemptedValue { get; }

    /// <summary>
    /// Gets the exception if one was thrown during conversion.
    /// </summary>
    public Exception? Exception { get; }

    /// <inheritdoc />
    public override void WriteMessages(TextWriter textWriter)
    {
        textWriter.WriteLine($"{Binding}: {ErrorMessage}");
    }

    private string ErrorMessage => Exception?.Message ?? $"cannot convert '{AttemptedValue}' to the expected type";
}