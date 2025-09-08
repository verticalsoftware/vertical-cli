using Vertical.Cli.Parsing;

namespace Vertical.Cli.Binding;

/// <summary>
/// Represents a binding context
/// </summary>
public interface IBindingContext
{
    /// <summary>
    /// Gets the input stream.
    /// </summary>
    TextReader InputStream { get; }

    /// <summary>
    /// Gets the parse result.
    /// </summary>
    ParseResult ParseResult { get; }
}