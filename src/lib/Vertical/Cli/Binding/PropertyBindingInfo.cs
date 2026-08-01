using Vertical.Cli.Configuration;
using Vertical.Cli.Conversion;
using Vertical.Cli.Parsing;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Binding;

/// <summary>
/// Contains data used for property binding.
/// </summary>
public sealed class PropertyBindingInfo
{
    internal PropertyBindingInfo()
    {
    }

    /// <summary>
    /// Gets the conversion provider.
    /// </summary>
    public required IConversionProvider ConversionProvider { get; init; }

    /// <summary>
    /// Gets the parse result.
    /// </summary>
    public required ParseResult ParseResult { get; init; }
    
    /// <summary>
    /// Gets application data.
    /// </summary>
    public required OptionsManager OptionsManager { get; init; }
    
    /// <summary>
    /// Gets the console abstraction input text reader.
    /// </summary>
    public required TextReader ConsoleInput { get; init; }
}