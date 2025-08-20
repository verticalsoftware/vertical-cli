using Vertical.Cli.Binding;
using Vertical.Cli.Configuration;
using Vertical.Cli.Invocation;

namespace Vertical.Cli.Validation;

/// <summary>
/// Represents the condition of a validation failing.
/// </summary>
public sealed class ValidationError : UsageError
{
    internal ValidationError(ISymbolBinding symbolBinding, object? attemptedValue, string message)
    {
        _symbolBinding = symbolBinding;
        AttemptedValue = attemptedValue;
        Message = message;
    }

    private readonly ISymbolBinding _symbolBinding;

    /// <summary>
    /// Gets the symbol that failed validation.
    /// </summary>
    public ISymbol Symbol => _symbolBinding;

    /// <summary>
    /// Gets the attempted value.
    /// </summary>
    public object? AttemptedValue { get; }

    /// <summary>
    /// Gets the message.
    /// </summary>
    public string Message { get; }

    /// <inheritdoc />
    public override void WriteMessages(TextWriter textWriter)
    {
        textWriter.WriteLine($"{Symbol}: {Message}");
    }
}