using Vertical.Cli.Binding;
using Vertical.Cli.Conversion;
using Vertical.Cli.Help;
using Vertical.Cli.Parsing;

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
    public IHelpProvider HelpProvider { get; set; } = DefaultHelpProvider.Instance;
    
    /// <summary>
    /// Gets or sets a function that is invoked when a command handler is not available.
    /// </summary>
    public Func<BindingContext, CancellationToken, Task<int>>? FallbackHandler { get; set; }
    
    /// <summary>
    /// Gets or sets whether to enable response files.
    /// </summary>
    public bool EnableResponseFiles { get; set; }

    /// <summary>
    /// Gets or sets a function that pre-processes/transforms arguments.
    /// </summary>
    /// <remarks>
    /// The value is a function that receives each input argument and returns a value the engine will
    /// use in its place. If the return value is <c>null</c>, the argument is removed from input to the
    /// engine. This function is called after response files are read before control enters the parsing
    /// operation.
    /// </remarks>
    public ArgumentPreProcessor[] ArgumentPreProcessors { get; set; } = [];
}
