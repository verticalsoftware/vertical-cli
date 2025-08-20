using Vertical.Cli.Binding;
using Vertical.Cli.Configuration;
using Vertical.Cli.Help;
using Vertical.Cli.Internal;
using Vertical.Cli.Parsing;

namespace Vertical.Cli.Invocation;

/// <summary>
/// Represents an object used to build a <see cref="HandlerRequest"/>
/// </summary>
public abstract class HandlerContextBuilder : ContextBuilder
{
    internal HandlerContextBuilder(ICommand command)
    {
        Command = command;
    }

    /// <summary>
    /// Gets the command the context is building.
    /// </summary>
    public ICommand Command { get; }
    
    /// <summary>
    /// Gets the list of model validators.
    /// </summary>
    public List<ModelConfiguration> ValidationTargets { get; } = [];

    /// <summary>
    /// Adds a model binder.
    /// </summary>
    /// <param name="modelBinder">The model binder instance.</param>
    public abstract void AddModelBinder(Delegate modelBinder);

    /// <summary>
    /// Adds a model constructor factory function.
    /// </summary>
    /// <param name="constructor">Constructor</param>
    public abstract void AddModelActivator(Delegate constructor);

    /// <summary>
    /// Builds the handler request.
    /// </summary>
    /// <param name="context">The <see cref="InvocationContext"/></param>
    /// <returns><see cref="HandlerRequest"/></returns>
    public abstract HandlerRequest Build(InvocationContext context);
}

/// <summary>
/// Represents an object used to build a <see cref="HandlerRequest"/> with a call site.
/// </summary>
public sealed class HandlerContextBuilder<TModel> : HandlerContextBuilder where TModel : class
{
    internal HandlerContextBuilder(ICommand command, CommandHandler<TModel> handler) : base(command)
    {
        _handler = handler;
    }

    private readonly CommandHandler<TModel> _handler;
    private ModelBinder<TModel>? _modelBinder;
    private Func<TModel>? _activator;

    /// <inheritdoc />
    public override void AddModelBinder(Delegate modelBinder) => _modelBinder = modelBinder as ModelBinder<TModel>;

    /// <inheritdoc />
    public override void AddModelActivator(Delegate constructor) => _activator = constructor as Func<TModel>;

    /// <inheritdoc />
    public override HandlerRequest Build(InvocationContext context)
    {
        // Parse result contains argument values mapped to symbols
        var parseResult = ParseResult.Create(
            context.Parser,
            context.TokenList,
            Symbols.ToArray(),
            context.Errors);
        
        var bindingContext = new BindingContext<TModel>(
            context.Configuration,
            Bindings,
            parseResult,
            context.Errors,
            _activator, context.Console.In);

        if (context.Errors.Count > 0)
        {
            return SetErrorState(context);
        }

        var model = ModelBinder(bindingContext);
        
        if (context.Errors.Count > 0)
        {
            return SetErrorState(context);
        }
        
        var errorCount = ValidationTargets
            .Aggregate(
                context.Errors.Count,
                (_, next) =>
                {
                    next.ValidateModel(model, context.Errors);
                    return context.Errors.Count;
                });
        
        // Don't send to command handler with errors
        if (errorCount > 0)
        {
            return SetErrorState(context);
        }

        return new HandlerRequest(InvokeCallSite);

        async Task<int> InvokeCallSite() => await _handler(model, context.CancellationToken);
    }

    private static HandlerRequest SetErrorState(InvocationContext context)
    {
        context.ExitCode = 150;
        return HandlerRequest.Default;
    }

    private ModelBinder<TModel> ModelBinder => _modelBinder ?? throw Exceptions.ModelBinderNotDefined(Command);
}