namespace Vertical.Cli.Invocation;

/// <summary>
/// Wraps a factory function for a command handler.
/// </summary>
/// <typeparam name="TModel">Handler model type</typeparam>
public class HandlerServiceProvider<TModel> : IAsyncDisposable where TModel : class
{
    private readonly Lazy<IHandler<TModel>> _lazyHandler;

    /// <summary>
    /// Initializes a new instance of the <see cref="HandlerServiceProvider{TModel}"/> class.
    /// </summary>
    /// <param name="handlerFactory">A factory function for the handler type.</param>
    public HandlerServiceProvider(Func<IHandler<TModel>> handlerFactory)
    {
        _lazyHandler = new Lazy<IHandler<TModel>>(handlerFactory);
    }

    /// <summary>
    /// Gets a handler instance.
    /// </summary>
    /// <returns><see cref="IHandler{TModel}"/></returns>
    public IHandler<TModel> Instance => _lazyHandler.Value;

    /// <inheritdoc />
    public virtual ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}