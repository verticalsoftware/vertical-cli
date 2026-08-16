using Vertical.Cli.Configuration;
using Vertical.Cli.Conversion;
using Vertical.Cli.Diagnostics;
using Vertical.Cli.Help;
using Vertical.Cli.Invocation;
using Vertical.Cli.Parsing;

namespace Vertical.Cli.Binding;

/// <summary>
/// Contains data used for property binding.
/// </summary>
public sealed class PropertyBindingInfo
{
    private readonly InvocationContext _context;
    private readonly IRootConfigurationView _configuration;

    internal PropertyBindingInfo(InvocationContext context)
    {
        _context = context;
        _configuration = _context.Configuration;
    }

    /// <summary>
    /// Gets the conversion provider.
    /// </summary>
    public IConversionProvider ConversionProvider => _configuration;

    /// <summary>
    /// Gets the help provider.
    /// </summary>
    internal IHelpProvider HelpProvider => _configuration.HelpOptions.HelpProvider;

    /// <summary>
    /// Gets the error list.
    /// </summary>
    internal List<CommandLineError> ErrorList => _context.Errors;

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