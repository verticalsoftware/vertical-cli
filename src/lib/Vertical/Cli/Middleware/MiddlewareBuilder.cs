using Vertical.Cli.Middleware.Components;

namespace Vertical.Cli.Middleware;

/// <summary>
/// Used to fluently configure middleware.
/// </summary>
public sealed class MiddlewareBuilder
{
    private readonly List<MiddlewareDelegate> _middleware;

    private static List<MiddlewareDelegate> DefaultComponents =>
    [
        HelpSystemMiddleware.InvokeAsync,
        DisplayHelpOptionSuggestionMiddleware.InvokeAsync,
        DisplayInputErrorsMiddleware.InvokeAsync,
        HandleDirectivesMiddleware.InvokeAsync,
        InjectResponseFileArgumentsMiddleware.InvokeAsync,
        HandleConsoleCancellationMiddleware.InvokeAsync
    ];

    internal MiddlewareBuilder(List<MiddlewareDelegate> middleware)
    {
        _middleware = middleware;
    }

    internal MiddlewareDelegate BuildPipeline() => _middleware
        .Aggregate((first, second) =>
            (context, next) =>
                first(context, ctx => second(ctx, next)));

    /// <summary>
    /// Creates an instance of the <see cref="MiddlewareBuilder"/> class with the default components.
    /// </summary>
    /// <returns></returns>
    public static MiddlewareBuilder CreateDefault() => new(DefaultComponents);

    /// <summary>
    /// Removes all delegates from the middleware list.
    /// </summary>
    /// <returns>A reference to this instance.</returns>
    public MiddlewareBuilder Clear()
    {
        _middleware.Clear();
        return this;
    }

    /// <summary>
    /// Adds middleware that displays help articles when the help option is invoked on a command.
    /// </summary>
    public MiddlewareBuilder DisplayHelpArticles => AddLast(HelpSystemMiddleware.InvokeAsync);

    /// <summary>
    /// Adds middleware that handles directive tokens.
    /// </summary>
    public MiddlewareBuilder EnableDirectives => AddLast(HandleDirectivesMiddleware.InvokeAsync);

    /// <summary>
    /// Adds middleware that listens for SIGTERM and SIGINT and invokes the internal
    /// cancellation source.
    /// </summary>
    public MiddlewareBuilder HandleConsoleCancellation => AddLast(HandleConsoleCancellationMiddleware.InvokeAsync);

    /// <summary>
    /// Adds middleware that display input errors to the output.
    /// </summary>
    public MiddlewareBuilder DisplayInputErrors => AddLast(DisplayInputErrorsMiddleware.InvokeAsync);

    /// <summary>
    /// Adds middleware that catches and displays application exceptions.
    /// </summary>
    public MiddlewareBuilder DisplayApplicationExceptions => AddLast(DisplayApplicationExceptionsMiddleware.InvokeAsync);

    /// <summary>
    /// Adds middleware that will inject arguments read from a response file stream to the token list.
    /// </summary>
    public MiddlewareBuilder InjectResponseFileArguments => AddLast(InjectResponseFileArgumentsMiddleware.InvokeAsync);

    /// <summary>
    /// Adds middleware that will display a suggestion to consult the help system for the target command
    /// when input errors are detected.
    /// </summary>
    public MiddlewareBuilder DisplayHelpOptionSuggestion => AddLast(DisplayHelpOptionSuggestionMiddleware.InvokeAsync);

    /// <summary>
    /// Adds a component to the start of the middleware pipeline.
    /// </summary>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>A reference to this instance.</returns>
    public MiddlewareBuilder AddFirst(MiddlewareDelegate middleware)
    {
        ArgumentNullException.ThrowIfNull(middleware);
        _middleware.Insert(0, middleware);
        return this;
    }

    /// <summary>
    /// Adds a component to the end of the middleware pipeline.
    /// </summary>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>A reference to this instance.</returns>
    public MiddlewareBuilder AddLast(MiddlewareDelegate middleware)
    {
        ArgumentNullException.ThrowIfNull(middleware);
        _middleware.Add(middleware);
        return this;
    }
}