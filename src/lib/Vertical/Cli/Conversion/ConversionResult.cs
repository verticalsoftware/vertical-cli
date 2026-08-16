using Vertical.Cli.Diagnostics;

namespace Vertical.Cli.Conversion;

/// <summary>
/// Describes the result of a value conversion.
/// </summary>
/// <typeparam name="TValue">Value type.</typeparam>
public sealed class ConversionResult<TValue>
{
    internal ConversionResult(
        TValue value,
        CommandLineError? error)
    {
        Value = value;
        Error = error;
    }

    /// <summary>
    /// Gets the conversion value.
    /// </summary>
    public TValue Value { get; }

    /// <summary>
    /// Gets whether the conversion succeeeded.
    /// </summary>
    public bool Success => Error is null;

    /// <summary>
    /// Gets the error that occurred during conversion.
    /// </summary>
    public CommandLineError? Error { get; }

    /// <inheritdoc />
    public override string ToString() => $"{typeof(TValue)}={Value}"
                                         + (!Success ? " (error)" : string.Empty);
}