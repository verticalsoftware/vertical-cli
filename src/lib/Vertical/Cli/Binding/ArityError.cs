using Vertical.Cli.Invocation;

namespace Vertical.Cli.Binding;

/// <summary>
/// Represents the condition that a parse result does not satisfy a symbol's arity.
/// </summary>
public sealed class ArityError : UsageError, ISymbolBindingError
{
    internal ArityError(ISymbolBinding symbolBinding, int receivedCount)
    {
        SymbolBinding = symbolBinding;
        ReceivedCount = receivedCount;
    }

    /// <summary>
    /// Gets the symbol binding that has an invalid parse arity.
    /// </summary>
    public ISymbolBinding SymbolBinding { get; }

    /// <summary>
    /// Gets the number of values received.
    /// </summary>
    public int ReceivedCount { get; }

    /// <inheritdoc />
    public override void WriteMessages(TextWriter textWriter)
    {
        switch (SymbolBinding.Arity, ReceivedCount)
        {
            case { ReceivedCount: 0 }:
                textWriter.WriteLine($"{SymbolBinding}: command requires use of this option or argument");
                break;
            
            case { ReceivedCount: > 1 }:
                textWriter.WriteLine($"{SymbolBinding}: single value expected but {ReceivedCount} received");
                break;
        }
    }
}