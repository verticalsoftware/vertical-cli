using Vertical.Cli.Configuration;
using Vertical.Cli.Conversion;
using Vertical.Cli.Diagnostics;
using Vertical.Cli.Help;
using Vertical.Cli.Invocation;
using Vertical.Cli.Parsing;
using Vertical.Cli.Utilities;

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
    /// Gets a read view of the application's options.
    /// </summary>
    public ApplicationData AppData => _configuration.ApplicationData;
    
    /// <summary>
    /// Gets the console abstraction input text reader.
    /// </summary>
    public required TextReader ConsoleInput { get; init; }
}