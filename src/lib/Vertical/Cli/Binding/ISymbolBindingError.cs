namespace Vertical.Cli.Binding;

/// <summary>
/// Represents an error that has an associated symbol binding.
/// </summary>
public interface ISymbolBindingError
{
    /// <summary>
    /// Gets the symbol binding.
    /// </summary>
    ISymbolBinding SymbolBinding { get; }
}