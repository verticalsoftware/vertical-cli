using Vertical.Cli.Conversion;
using Vertical.Cli.Help;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Defines parsing and binding options.
/// </summary>
public sealed class CliOptions
{
    internal CliOptions()
    {
    }
    
    /// <summary>
    /// Gets the collection of value converters to use.
    /// </summary>
    public List<ValueConverter> ValueConverters { get; } = [];

    /// <summary>
    /// Gets whether to ignore arguments that don't match to symbols
    /// configured in the command (will not bypass model bindings).
    /// </summary>
    public bool IgnoreUnmappedArguments { get; set; } = false;
    
    /// <summary>
    /// Gets the object that creates help content.
    /// </summary>
    public IHelpProvider? HelpProvider { get; set; }
}