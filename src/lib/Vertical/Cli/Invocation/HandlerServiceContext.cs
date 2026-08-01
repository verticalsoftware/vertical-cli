using Vertical.Cli.Diagnostics;

namespace Vertical.Cli.Invocation;

internal static class HandlerServiceContext
{
    public static HandlerServiceContext<TModel> Create<TModel>(
        InvocationContext context,
        Func<TModel, CancellationToken, Task<int>> handler)
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(handler);

        return new HandlerServiceContext<TModel>(context, _ => new DelegatedHandler<TModel>(handler));
    }

    public static HandlerServiceContext<TModel> Create<TModel>(
        InvocationContext context,
        Func<IServiceProvider?, IHandler<TModel>> handlerResolver)
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(handlerResolver);

        return new HandlerServiceContext<TModel>(context, handlerResolver);
    }

    public static HandlerServiceContext<TModel> Create<TModel, THandler>(InvocationContext context)
        where TModel : class
        where THandler : class, IHandler<TModel>
    {
        ArgumentNullException.ThrowIfNull(context);

        return new HandlerServiceContext<TModel>(
            context,
            serviceProvider =>
            {
                if (serviceProvider is null)
                {
                    throw Exceptions.ServiceProviderNotConfigured();
                }

                return serviceProvider.GetService(typeof(THandler)) as THandler
                       ?? throw Exceptions.CommandHandlerNotResolved(typeof(THandler));
            });
    }
}

internal sealed class HandlerServiceContext<TModel> : IAsyncDisposable where TModel : class
{
    private readonly InvocationContext _context;
    private readonly Func<IServiceProvider?, IHandler<TModel>> _handlerFactory;
    private readonly Lazy<IServiceProvider?> _lazyServiceProvider;

    public HandlerServiceContext(
        InvocationContext context,
        Func<IServiceProvider?, IHandler<TModel>> handlerFactory)
    {
        _context = context;
        _handlerFactory = handlerFactory;
        _lazyServiceProvider = new Lazy<IServiceProvider?>(() => context
            .ServiceContext
            .ServiceProviderFactory?
            .Invoke(context));
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        switch (provider: _lazyServiceProvider, context: _context.ServiceContext)
        {
            case { provider.IsValueCreated: false }:
            case { provider.Value: null }:                
            case { context.Dispose: false }:                
                break;
            
            // ReSharper disable once SuspiciousTypeConversion.Global
            case { provider.Value: IAsyncDisposable asyncDisposable }:
                await asyncDisposable.DisposeAsync();
                break;
            
            case { provider.Value: IDisposable disposable }:
                disposable.Dispose();
                break;
        }
    }
    
    public IHandler<TModel> GetHandler() => _handlerFactory(_lazyServiceProvider.Value);
}