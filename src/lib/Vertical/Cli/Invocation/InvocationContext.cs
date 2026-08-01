using Vertical.Cli.Configuration;
using Vertical.Cli.Diagnostics;
using Vertical.Cli.IO;
using Vertical.Cli.Parsing;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Invocation;

/// <summary>
/// Represents runtime data available to the middleware pipeline.
/// </summary>
public sealed class InvocationContext : IDisposable
{
    private readonly RootConfiguration _configuration;
    private readonly CancellationTokenSource _cancelSource = new();

    internal InvocationContext(RootConfiguration configuration, string[] arguments)
    {
        Arguments = arguments;
        TokenList = new TokenList(arguments);
        RootCommand = configuration.RootCommand;
        OutputWriter = new OutputWriter(configuration.Console, configuration.OutputFormatter);
        
        _configuration = configuration;
    }

    /// <summary>
    /// Gets whether the context is in a routable state.
    /// </summary>
    public bool IsInRoutableState => Errors.Count == 0 && !Result.HasValue;

    /// <summary>
    /// Gets the output writer.
    /// </summary>
    public OutputWriter OutputWriter { get; set; }

    /// <summary>
    /// Gets the options manager.
    /// </summary>
    public OptionsManager ApplicationOptions => _configuration.OptionsManager;

    /// <summary>
    /// Releases all resources used by this component.
    /// </summary>
    public void Dispose() => _cancelSource.Dispose();

    /// <summary>
    /// Gets a token that can be observed for cancellation.
    /// </summary>
    public CancellationToken CancellationToken => _cancelSource.Token;

    /// <summary>
    /// Gets a view of the configuration.
    /// </summary>
    public IRootConfigurationView Configuration => _configuration;

    /// <summary>
    /// Gets the provided application arguments.
    /// </summary>
    public string[] Arguments { get; }

    /// <summary>
    /// Gets the token list.
    /// </summary>
    public TokenList TokenList { get; }
    
    /// <summary>
    /// Gets the root command.
    /// </summary>
    public RootCommand RootCommand { get; }

    /// <summary>
    /// Gets the list of command line errors.
    /// </summary>
    public List<CommandLineError> Errors { get; } = [];

    /// <summary>
    /// Gets or sets the result code returned to the application.
    /// </summary>
    public int? Result { get; set; }

    /// <summary>
    /// Throws an exception if any errors are present.
    /// </summary>
    /// <exception cref="CommandLineException"></exception>
    public void AssertState()
    {
        if (Errors.Count == 0) return;
        throw new CommandLineException(Errors);
    }

    /// <summary>
    /// Adds an error to the internal list.
    /// </summary>
    /// <param name="error"></param>
    public void AddError(CommandLineError error)
    {
        ArgumentNullException.ThrowIfNull(error);
        Errors.Add(error);
    }

    /// <summary>
    /// Adds any errors to the internal list.
    /// </summary>
    /// <param name="errors">The errors to add.</param>
    /// <returns>The current count of errors after compeltion of the operation.</returns>
    public int AddErrors(IEnumerable<CommandLineError> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);
        Errors.AddRange(errors);
        return Errors.Count;
    }

    internal ServiceContext ServiceContext => _configuration.ServiceContext;

    public void RequestCancel() => _cancelSource.Cancel();
}