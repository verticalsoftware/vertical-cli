using Vertical.Cli.Configuration;
using Vertical.Cli.Internal;
using Vertical.Cli.IO;
using Vertical.Cli.Parsing;

namespace Vertical.Cli.Invocation;

/// <summary>
/// Represents the context by which a command line application is run.
/// </summary>
public sealed class InvocationContext : IDisposable
{
    private InvocationContext()
    {
    }

    private int? _exitCode;

    /// <summary>
    /// Gets the root command.
    /// </summary>
    public IRootCommand RootCommand => Configuration.RootCommand;

    /// <summary>
    /// Releases resources used by the component.
    /// </summary>
    public void Dispose() => CancellationSource.Dispose();

    /// <summary>
    /// Gets the cancellation token source.
    /// </summary>
    public CancellationTokenSource CancellationSource { get; } = new();

    /// <summary>
    /// Gets the cancellation token.
    /// </summary>
    public CancellationToken CancellationToken => CancellationSource.Token;
    
    /// <summary>
    /// Gets the parser.
    /// </summary>
    public required IParser Parser { get; init; }
    
    /// <summary>
    /// Gets a list of tokens parsed from the client's arguments.
    /// </summary>
    public required LinkedList<Token> TokenList { get; init; }
    
    /// <summary>
    /// Gets the root configuration.
    /// </summary>
    public required IRootConfiguration Configuration { get; init; }
    
    /// <summary>
    /// Gets the original string arguments.
    /// </summary>
    public required string[] Arguments { get; init; }
    
    /// <summary>
    /// Gets the tokens that were initially parsed before any manipulation by middleware.
    /// </summary>
    public required Token[] OriginalTokens { get; init; }
    
    /// <summary>
    /// Gets the console abstraction.
    /// </summary>
    public required IConsole Console { get; init; }

    /// <summary>
    /// Gets the list of <see cref="UsageError"/> reported.
    /// </summary>
    public List<UsageError> Errors { get; } = [];

    /// <summary>
    /// Gets or sets the exit code.
    /// </summary>
    public int ExitCode
    {
        get => _exitCode ?? 0;
        set => _exitCode ??= value;
    }

    /// <summary>
    /// Gets whether an exit code has been set.
    /// </summary>
    public bool IsExitCodeSet => _exitCode.HasValue;
    
    /// <summary>
    /// Gets the target command.
    /// </summary>
    /// <param name="dequeue"><c>true</c> to dequeue tokens matching command names</param>
    /// <returns>The command and the next immediate node</returns>
    public (ICommand Command, LinkedListNode<Token>? NextNode) GetTargetCommand(bool dequeue = false)
    {
        ICommand command = RootCommand;
        var node = TokenList.First;

        for (;
             node?.Value is { } token &&
             command.Commands.FirstOrDefault(child => child.Name.Equals(token.Text)) is { } subCommand;)
        {
            command = subCommand;
            node = dequeue ? TokenList.Dequeue(node) : node.Next;
        }
         
        return (command, node);
    }

    internal static InvocationContext Create(
        IRootConfiguration configuration,
        string[] arguments,
        IConsole console)
    {
        var parser = configuration.CreateParser();
        var tokens = parser.ParseArguments(arguments).ToArray();
        var tokenList = new LinkedList<Token>(tokens);
        
        return new InvocationContext
        {
            Parser = parser,
            OriginalTokens = tokens,
            TokenList = tokenList,
            Configuration = configuration,
            Arguments = arguments,
            Console = console
        };
    }
    
    internal static async Task<int> InvokeAsync(
        IRootConfiguration configuration,
        string[] arguments,
        IConsole console)
    {
        using var context = Create(configuration, arguments, console);
        
        // Build & invoke pipeline
        var pipeline = configuration.CreateMiddlewarePipeline(StaticMiddleware.GetDelegates());
        await pipeline(context);

        return context.ExitCode;
    }
}