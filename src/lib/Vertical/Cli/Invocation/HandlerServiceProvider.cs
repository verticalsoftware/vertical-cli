namespace Vertical.Cli.Invocation;

/// <summary>
/// Wraps a factory function for a command handler.
/// </summary>
/// <typeparam name="TModel">Handler model type</typeparam>
public class HandlerServiceProvider<TModel> : IAsyncDisposable where TModel : class
{
    private readonly Func<IHandler<TModel>> _handlerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="HandlerServiceProvider{TModel}"/> class.
    /// </summary>
    /// <param name="handlerFactory">A factory function for the handler type.</param>
    public HandlerServiceProvider(Func<IHandler<TModel>> handlerFactory)
    {
        _handlerFactory = handlerFactory ?? throw new ArgumentNullException(nameof(handlerFactory));
    }

    /// <summary>
    /// Gets a handler instance.
    /// </summary>
    /// <returns><see cref="IHandler{TModel}"/></returns>
    public IHandler<TModel> GetInstance() => _handlerFactory();

    /// <inheritdoc />
    public virtual ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}