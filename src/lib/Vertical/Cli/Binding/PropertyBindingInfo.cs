using Vertical.Cli.Configuration;
using Vertical.Cli.Conversion;
using Vertical.Cli.Parsing;

namespace Vertical.Cli.Binding;

/// <summary>
/// Contains data used for property binding.
/// </summary>
public sealed class PropertyBindingInfo
{
    private readonly IRootConfigurationView _configuration;

    internal PropertyBindingInfo(IRootConfigurationView configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the conversion provider.
    /// </summary>
    public IConversionProvider ConversionProvider => _configuration;

    /// <summary>
    /// Gets the parse result.
    /// </summary>
    public required ParseResult ParseResult { get; init; }

    /// <summary>
    /// Gets application defined options.
    /// </summary>
    /// <typeparam name="TOptions">Options type.</typeparam>
    /// <returns>The singleton options instance.</returns>
    public TOptions GetOptions<TOptions>() where TOptions : class, new()  => 
        _configuration.OptionsManager.GetOptions<TOptions>();
    
    /// <summary>
    /// Gets the console abstraction input text reader.
    /// </summary>
    public required TextReader ConsoleInput { get; init; }
}