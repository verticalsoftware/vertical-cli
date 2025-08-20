using System.Reflection.Metadata;
using Vertical.Cli.Configuration;
using Vertical.Cli.Directives;
using Vertical.Cli.Help;
using Vertical.Cli.Internal;
using Vertical.Cli.Parsing;
using Vertical.Cli.ResponseFiles;
using Vertical.Cli.Utilities;

namespace Vertical.Cli.Invocation;

/// <summary>
/// Represents an object used to configure the middleware pipeline.
/// </summary>
public sealed class MiddlewareConfiguration
{
    internal MiddlewareConfiguration(CommandLineOptions options)
    {
        _options = options;
        

    }

    private readonly CommandLineOptions _options;

    private readonly List<MiddlewareHandler> _middleware = [];

    internal Func<InvocationContext, Task> BuildPipeline(IEnumerable<Middleware> terminalMiddleware)
    {
        var pipeline = _middleware
            .Select(entry => entry.Action)
            .Concat(terminalMiddleware)
            .Build();

        return context => pipeline(context, _ => Task.CompletedTask);
    }

    /// <summary>
    /// Adds default middleware components.
    /// </summary>
    /// <returns>A reference to this instance.</returns>
    public MiddlewareConfiguration UseDefaults()
    {
        HandleExceptions();
        DisplayHelpOnError();
        HandleErrors();
        HandleCancellation();
        AddResponseFiles();
        AddHelpOption();
        AddVersionOption();

        return this;
    }

    /// <summary>
    /// Adds a new middleware.
    /// </summary>
    /// <param name="middleware">The middleware implementation.</param>
    /// <returns>A reference to this instance</returns>
    public MiddlewareConfiguration Add(Middleware middleware) => Add(
        Guid.NewGuid(),
        (_, id) => new MiddlewareHandler(middleware, id));

    /// <summary>
    /// Removes all middleware.
    /// </summary>
    /// <returns>A reference to this instance.</returns>
    public MiddlewareConfiguration Clear()
    {
        _middleware.Clear();
        return this;
    }

    /// <summary>
    /// Removes a default component from the pipeline.
    /// </summary>
    /// <param name="builtInType">The default middleware component to remove.</param>
    /// <returns>A reference to this instance</returns>
    /// <remarks>This method never fails.</remarks>
    public MiddlewareConfiguration Remove(BuiltInMiddleware builtInType)
    {
        var index = _middleware.FindIndex(m => m.RegistrationId == builtInType.Id);

        if (index == -1)
            return this;
        
        _middleware.RemoveAt(index);
        return this;
    }

    /// <summary>
    /// Replaces a default component with a custom implementation.
    /// </summary>
    /// <param name="builtInType">The built in type to replace.</param>
    /// <param name="middleware">The custom implementation to substitute.</param>
    /// <returns>A reference to this instance.</returns>
    public MiddlewareConfiguration Replace(
        BuiltInMiddleware builtInType,
        Middleware middleware)
    {
        var index = _middleware.FindIndex(m => m.RegistrationId == builtInType.Id);

        if (index == -1)
        {
            return Add(builtInType.Id, (_, id) => new MiddlewareHandler(middleware, id));
        }

        _middleware[index] = new MiddlewareHandler(middleware, builtInType.Id);
        return this;
    }

    /// <summary>
    /// Adds a function that evaluates directive tokens.
    /// </summary>
    /// <param name="handler">The function that evaluates the token and takes action.</param>
    /// <param name="helpTag">The help tag to associate with the directive.</param>
    /// <returns>A reference to this instance</returns>
    public MiddlewareConfiguration AddDirectiveHandler(
        Func<DirectiveContext, Task> handler,
        DirectiveHelpTag? helpTag = null)
    {
        return AddDirectiveHandler(
            Guid.NewGuid(),
            state: 0,
            handler,
            helpTag);
    }

    /// <summary>
    /// Adds middleware that injects tokens from response files.
    /// </summary>
    /// <returns>A reference to this instance.</returns>
    public MiddlewareConfiguration AddResponseFiles()
    {
        return Add(
            BuiltInMiddleware.ResponseFiles.Id,
            (handler, id) => handler ?? new MiddlewareHandler(Handle, id));
        
        async Task Handle(InvocationContext context, Func<InvocationContext, Task> next)
        {
            await ResponseFileParser.ParseResponseFileTokensAsync(context, File.OpenRead);
            await next(context);
        }
    }

    /// <summary>
    /// Adds a function that evaluates directive tokens.
    /// </summary>
    /// <param name="state">The state to provide to the handler.</param>
    /// <param name="handler">The function that evaluates the token and takes action.</param>
    /// <param name="helpTag">The help tag to associate with the directive.</param>
    /// <typeparam name="TState">State type</typeparam>
    /// <returns>A reference to this instance</returns>
    public MiddlewareConfiguration AddDirectiveHandler<TState>(
        TState state,
        Func<DirectiveContext<TState>, Task> handler,
        DirectiveHelpTag? helpTag = null)
    {
        return AddDirectiveHandler(
            Guid.NewGuid(),
            state,
            handler,
            helpTag);
    }

    /// <summary>
    /// Adds middleware that invokes the help system if the configured symbol is matched.
    /// </summary>
    /// <returns>A reference to this instance.</returns>
    public MiddlewareConfiguration AddHelpOption()
    {
        return AddAncillaryOption(
            BuiltInMiddleware.HelpOption.Id,
            AncillaryOptionKind.Help,
            _options.HelpOptionAliases,
            _options.HelpOptionHelpTag,
            async (context, command) =>
            {
                await context.InvokeHelpAsync(command, _options.HelpProviderFactory);
                return 0;
            });
    }

    /// <summary>
    /// Adds middleware that prints the application version if the configured symbol is matched.
    /// </summary>
    /// <returns>A reference to this instance.</returns>
    public MiddlewareConfiguration AddVersionOption()
    {
        return AddAncillaryOption(
            BuiltInMiddleware.VersionOption.Id,
            AncillaryOptionKind.Version,
            _options.VersionOptionAliases,
            _options.VersionOptionHelpTag,
            Handle);

        Task<int> Handle(InvocationContext context, ICommand _)
        {
            var displayVersion = _options.ApplicationVersion ?? "not available";
                
            context.Console.WriteLine($"{context.RootCommand.Name}, version {displayVersion}");
            return Task.FromResult(0);
        }
    }

    /// <summary>
    /// Adds middleware that signals the application's cancellation token.
    /// </summary>
    public MiddlewareConfiguration HandleCancellation()
    {
        return Add(BuiltInMiddleware.HandleCancellation.Id, (handler, id) => handler ?? new MiddlewareHandler(Handle, id));
        
        static Task Handle(InvocationContext context, Func<InvocationContext, Task> next)
        {
            context.Console.HandleCancelEvent(() =>
            {
                context.CancellationSource.Cancel();
                context.ExitCode = 143;
            });

            return next(context);
        }
    }

    /// <summary>
    /// Adds middleware to handle an intercepting option.
    /// </summary>
    /// <param name="aliases">The aliases the option is identified by.</param>
    /// <param name="handleOption">The function that handles the logic of the option.</param>
    /// <param name="helpTag">Optional help content to associate with the option.</param>
    /// <returns></returns>
    public MiddlewareConfiguration AddAncillaryOption(string[] aliases,
        Func<InvocationContext, ICommand, Task<int>> handleOption,
        object? helpTag)
    {
        return AddAncillaryOption(
            Guid.NewGuid(),
            AncillaryOptionKind.Application,
            aliases,
            helpTag,
            handleOption);
    }

    /// <summary>
    /// Adds middleware that catches and displays exceptions that occur in the pipeline.
    /// </summary>
    /// <returns>A reference to this instance</returns>
    public MiddlewareConfiguration HandleExceptions()
    {
        return Add(
            BuiltInMiddleware.HandleExceptions.Id,
            (handler, id) => handler ?? new MiddlewareHandler(Handle, id)
        );

       static async Task Handle(InvocationContext context, Func<InvocationContext, Task> next)
        {
            try
            {
                await next(context);
            }
            catch (Exception exception)
            {
                context.Console.WriteErrorLine(exception.ToString());
            }
        }
    }

    /// <summary>
    /// Adds middleware that catches and displays exceptions that occur in the pipeline.
    /// </summary>
    /// <returns>A reference to this instance</returns>
    public MiddlewareConfiguration HandleErrors()
    {
        return Add(
            BuiltInMiddleware.HandleErrors.Id,
            (handler, id) => handler ?? new MiddlewareHandler(Handle, id)
        );

        static async Task Handle(InvocationContext context, Func<InvocationContext, Task> next)
        {
            await next(context);

            if (context.Errors.Count == 0)
                return;

            var (textWriter, errorMode) = (context.Console.Out, context.Console.ErrorMode);
            context.Console.ErrorMode = true;

            try
            {
                foreach (var error in context.Errors)
                {
                    error.WriteMessages(textWriter);
                }
            }
            finally
            {
                context.Console.ErrorMode = errorMode;
            }
        }
    }

    /// <summary>
    /// Adds middleware that detects when a usage error has occurred, and displays contextual
    /// help automatically.
    /// </summary>
    /// <returns>A reference to this instance.</returns>
    public MiddlewareConfiguration DisplayHelpOnError()
    {
        return Add(
            BuiltInMiddleware.AutoDisplayHelp.Id,
            (handler, id) => handler ?? new MiddlewareHandler(Handle, id));
        
        async Task Handle(InvocationContext context, Func<InvocationContext, Task> next)
        {
            var target = context.GetTargetCommand();
            
            await next(context);

            if (context.Errors.Count == 0)
                return;

            context.Console.WriteLine();
            
            await context.InvokeHelpAsync(
                target.Command,
                _options.HelpProviderFactory);
        }
    }

    internal void ConfigureContext(IContextBuilder builder)
    {
        foreach (var handler in _middleware)
        {
            handler.AppendSymbols(builder);
        }
    }

    private MiddlewareConfiguration AddAncillaryOption(
        Guid registrationId,
        AncillaryOptionKind kind,
        string[] aliases,
        object? helpTag,
        Func<InvocationContext, ICommand, Task<int>> handleOption)
    {
        return Add(
            registrationId,
            (handler, id) => handler ?? new MiddlewareHandler(TryHandleSymbol, id, ConfigureBuilder)
        );
        
        void ConfigureBuilder(IContextBuilder builder)
        {
            builder.AddSymbols([new AncillaryOptionSymbol(kind, aliases, helpTag)]);
        }

        async Task TryHandleSymbol(InvocationContext context, Func<InvocationContext, Task> next)
        {
            var target = context.GetTargetCommand(dequeue: false);
            
            for (var node = context.TokenList.First; node != null; node = node.Next)
            {
                if (!(node.Value is { Kind: TokenKind.OptionSymbol } token &&
                      aliases.Any(alias => alias.Equals(token.Text))))
                {
                    continue;
                }
                
                context.ExitCode = await handleOption(context, target.Command);
                return;
            }

            await next(context);
        }
    }

    private MiddlewareConfiguration AddDirectiveHandler<TState>(
        Guid registrationId,
        TState state,
        Func<DirectiveContext<TState>, Task> directiveHandler,
        DirectiveHelpTag? helpTag)
    {
        return Add(registrationId, (handler, id) => handler ?? 
                                                    new MiddlewareHandler(MatchDirective, id, ConfigureBuilder));

        async Task MatchDirective(InvocationContext context, Func<InvocationContext, Task> next)
        {
            for (var node = context.TokenList.First; node?.Value is { Kind: TokenKind.Directive };)
            {
                var directiveArgs = new DirectiveContext<TState>(node.Value, state, context);
                await directiveHandler(directiveArgs);

                if (context.IsExitCodeSet)
                    return;

                node = directiveArgs.Dequeue
                    ? context.TokenList.Dequeue(node)
                    : node.Next;
            }

            await next(context);
        }
        
        void ConfigureBuilder(IContextBuilder builder)
        {
            if (helpTag == null)
                return;
            
            builder.DirectiveHelpTags.Add(helpTag);
        }
    }

    private MiddlewareConfiguration Add(
        Guid id, 
        Func<MiddlewareHandler?, Guid, MiddlewareHandler> factory)
    {
        for (var c = 0; c < _middleware.Count; c++)
        {
            if (_middleware[c].RegistrationId != id)
                continue;

            _middleware[c] = factory(_middleware[c], id);
            return this;
        }
        
        _middleware.Add(factory(null, id));
        return this;
    }
}