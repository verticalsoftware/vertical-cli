using Vertical.Cli.Help;
using Vertical.Cli.Invocation;

namespace Vertical.Cli.Configuration;

/// <summary>
/// Represents a command that has a handler implementation.
/// </summary>
/// <typeparam name="TModel">Model type</typeparam>
public abstract class InvokableCommand<TModel> : ContainerCommand, IInvocationTarget where TModel : class
{
    /// <inheritdoc />
    protected InvokableCommand(string name, 
        CommandHandler<TModel> handler,
        CommandHelpTag? helpTag) : base(name, helpTag)
    {
        _handler = handler;
    }

    private readonly CommandHandler<TModel> _handler;

    /// <inheritdoc />
    public virtual HandlerContextBuilder CreateRequestBuilder(
        IRootConfiguration configuration, 
        IModelConfigurationFactory modelConfigurationFactory)
    {
        var handlerRequestBuilder = new HandlerContextBuilder<TModel>(this, _handler);
        
        return configuration.ConfigureRequestBuilder(
            handlerRequestBuilder, 
            typeof(TModel), modelConfigurationFactory);
    }
}