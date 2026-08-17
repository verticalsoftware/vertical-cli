using Vertical.Cli.Configuration;
using Vertical.Cli.Help;
using Vertical.Cli.Invocation;
using Vertical.Cli.Middleware.Components;
using Vertical.Cli.Validation;

namespace Vertical.Cli.Middleware;

/// <summary>
/// Used to fluently configure middleware.
/// </summary>
public sealed class MiddlewareBuilder
{
    private readonly List<MiddlewareDelegate> _middleware = [];
    private readonly HashSet<Type> _delegateTypes = [];
    private readonly List<MiddlewareSymbol> _symbols = [];

    private static List<MiddlewareDelegate> DefaultComponents =>
    [
        HandleDirectiveSymbolHooksMiddleware.InvokeAsync,
        HandleMiddlewareSwitchesMiddleware.InvokeAsync,
        HelpSystemMiddleware.InvokeAsync,
        DisplayHelpOptionSuggestionMiddleware.InvokeAsync,
        DisplayInputErrorsMiddleware.InvokeAsync,
        InjectResponseFileArgumentsMiddleware.InvokeAsync,
        HandleConsoleCancellationMiddleware.InvokeAsync
    ];

    private MiddlewareBuilder()
    {
    }

    internal MiddlewareDelegate BuildPipeline() => _middleware
        .Aggregate((first, second) =>
            (context, next) =>
                first(context, ctx => second(ctx, next)));

    internal IReadOnlyList<MiddlewareSymbol> Symbols => _symbols;

    /// <summary>
    /// Creates an instance of the <see cref="MiddlewareBuilder"/> class with the default components.
    /// </summary>
    /// <returns>A reference to this instance.</returns>
    public static MiddlewareBuilder CreateDefault() => new MiddlewareBuilder()
        .HandleDirectives()
        .HandleSwitches()
        .DisplayHelpArticles()
        .DisplayHelpOptionSuggestion()
        .DisplayInputErrors()
        .InjectResponseFileArguments()
        .HandleConsoleCancellation();

    internal bool IsRegistered(Type type) => _delegateTypes.Contains(type);

    /// <summary>
    /// Adds a directive.
    /// </summary>
    /// <param name="identifier">The identifier for the directive.</param>
    /// <param name="handler">A delegate that evaluates and/or manipulates the current context.</param>
    /// <param name="helpTopic">Optional help topic.</param>
    /// <returns>A reference to this instance.</returns>
    public MiddlewareBuilder AddDirective(
        string identifier,
        Func<InvocationContext, Task> handler,
        HelpTopic? helpTopic = null)
    {
        _symbols.Add(new MiddlewareDirective(identifier, handler, helpTopic));
        return this;
    }

    /// <summary>
    /// Adds a directive.
    /// </summary>
    /// <param name="identifier">The identifier for the directive.</param>
    /// <param name="handler">A delegate that evaluates and/or manipulates the current context.</param>
    /// <param name="validate">A delegate that validates the parameter value.</param>
    /// <param name="helpTopic">Optional help topic.</param>
    /// <param name="useDefault">A function that provides the default parameter value.</param>
    /// <returns>A reference to this instance.</returns>
    public MiddlewareBuilder AddDirective<TValue>(
        string identifier,
        Func<ParameterizedMiddlewareDirectiveInfo<TValue>, Task> handler,
        Func<TValue>? useDefault = null,
        Action<IValidationEventInfo<InvocationContext, TValue>>? validate = null,
        HelpTopic? helpTopic = null)
    {
        _symbols.Add(new ParameterizedMiddlewareDirective<TValue>(
            identifier,
            handler,
            useDefault,
            validate,
            helpTopic));

        return this;
    }
    
    /// <summary>
    /// Adds a middleware switch.
    /// </summary>
    /// <param name="identifier">Unique switch identifier.</param>
    /// <param name="aliasList">One or more aliases the switch can be referred to by.</param>
    /// <param name="handler">A delegate that handles the switch implementation and returns an exit code.</param>
    /// <param name="helpTopic">The help topic to associate with the switch.</param>
    /// <returns></returns>
    public MiddlewareBuilder AddSwitch(
        string identifier,
        AliasList aliasList,
        Func<InvocationContext, Task<int?>> handler,
        HelpTopic? helpTopic = null)
    {
        _symbols.Add(new MiddlewareSwitch(identifier, aliasList.GetValues(), handler, helpTopic));
        return this;
    }

    /// <summary>
    /// Removes all delegates from the middleware list.
    /// </summary>
    /// <returns>A reference to this instance.</returns>
    public MiddlewareBuilder Clear()
    {
        _middleware.Clear();
        _delegateTypes.Clear();
        return this;
    }

    /// <summary>
    /// Adds middleware that evaluates directive tokens matched in the token list.
    /// </summary>
    public MiddlewareBuilder HandleDirectives() => AddLast(
        HandleDirectiveSymbolHooksMiddleware.InvokeAsync,
        typeof(HandleDirectiveSymbolHooksMiddleware));

    /// <summary>
    /// Adds middleware that evaluates global switch symbols matched in the token list.
    /// </summary>
    public MiddlewareBuilder HandleSwitches() => AddLast(
        HandleMiddlewareSwitchesMiddleware.InvokeAsync,
        typeof(HandleMiddlewareSwitchesMiddleware));

    /// <summary>
    /// Adds middleware that displays help articles when the help option is invoked on a command.
    /// </summary>
    public MiddlewareBuilder DisplayHelpArticles() => AddLast(
        HelpSystemMiddleware.InvokeAsync,
        typeof(HelpSystemMiddleware));
    
    /// <summary>
    /// Adds middleware that listens for SIGTERM and SIGINT and invokes the internal
    /// cancellation source.
    /// </summary>
    public MiddlewareBuilder HandleConsoleCancellation() => AddLast(
        HandleConsoleCancellationMiddleware.InvokeAsync,
        typeof(HandleConsoleCancellationMiddleware));

    /// <summary>
    /// Adds middleware that display input errors to the output.
    /// </summary>
    public MiddlewareBuilder DisplayInputErrors() => AddLast(
        DisplayInputErrorsMiddleware.InvokeAsync,
        typeof(DisplayInputErrorsMiddleware));

    /// <summary>
    /// Adds middleware that catches and displays application exceptions.
    /// </summary>
    public MiddlewareBuilder DisplayApplicationExceptions() => AddLast(
        DisplayApplicationExceptionsMiddleware.InvokeAsync,
        typeof(DisplayApplicationExceptionsMiddleware));

    /// <summary>
    /// Adds middleware that will inject arguments read from a response file stream to the token list.
    /// </summary>
    public MiddlewareBuilder InjectResponseFileArguments() => AddLast(
        InjectResponseFileArgumentsMiddleware.InvokeAsync,
        typeof(InjectResponseFileArgumentsMiddleware));

    /// <summary>
    /// Adds middleware that will display a suggestion to consult the help system for the target command
    /// when input errors are detected.
    /// </summary>
    public MiddlewareBuilder DisplayHelpOptionSuggestion() => AddLast(
        DisplayHelpOptionSuggestionMiddleware.InvokeAsync,
        typeof(DisplayHelpOptionSuggestionMiddleware));

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

    private MiddlewareBuilder AddLast(MiddlewareDelegate middleware, Type type)
    {
        _middleware.Add(middleware);
        _delegateTypes.Add(type);
        return this;
    }
}