using Vertical.Cli.Invocation;

namespace Vertical.Cli.Binding;

/// <summary>
/// Represents when the client did not provide a value for an option.
/// </summary>
public sealed class MissingParameterError : UsageError, ISymbolBindingError
{
    internal MissingParameterError(ISymbolBinding symbolBinding)
    {
        SymbolBinding = symbolBinding;
    }

    /// <summary>
    /// Gets the symbol binding that is missing a parameter.
    /// </summary>
    public ISymbolBinding SymbolBinding { get; }

    /// <inheritdoc />
    public override void WriteMessages(TextWriter textWriter)
    {
        textWriter.WriteLine($"{SymbolBinding}: requires parameter value");
    }
}