namespace Vertical.Cli.Invocation;

/// <summary>
/// Encapsulates a handler service.
/// </summary>
/// <typeparam name="TModel">Handler model type.</typeparam>
public class HandlerService<TModel> : IAsyncDisposable where TModel : class
{
    private readonly Lazy<IHandler<TModel>> _lazyHandler;

    /// <summary>
    /// Initializes a new instance of the <see cref="HandlerService{TModel}"/> class.
    /// </summary>
    /// <param name="handlerFactory">A method that creates the handler instance.</param>
    public HandlerService(Func<IHandler<TModel>> handlerFactory)
    {
        _lazyHandler = new Lazy<IHandler<TModel>>(handlerFactory);
    }

    /// <summary>
    /// Gets the handler instance.
    /// </summary>
    /// <returns></returns>
    public IHandler<TModel> GetHandler() => _lazyHandler.Value;

    /// <inheritdoc />
    public virtual ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}