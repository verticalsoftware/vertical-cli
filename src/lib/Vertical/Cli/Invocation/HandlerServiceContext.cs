using Vertical.Cli.Diagnostics;

namespace Vertical.Cli.Invocation;

internal static class HandlerServiceContext
{
    public static HandlerServiceContext<TModel> Create<TModel>(
        ServiceContext serviceContext,
        Func<TModel, CancellationToken, Task<int>> handler)
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(serviceContext);
        ArgumentNullException.ThrowIfNull(handler);

        return new HandlerServiceContext<TModel>(
            serviceContext,
            _ => new DelegatedHandler<TModel>(handler));
    }

    public static HandlerServiceContext<TModel> Create<TModel>(
        ServiceContext serviceContext,
        Func<IServiceProvider?, IHandler<TModel>> handlerResolver)
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(serviceContext);
        ArgumentNullException.ThrowIfNull(handlerResolver);

        return new HandlerServiceContext<TModel>(serviceContext, handlerResolver);
    }

    public static HandlerServiceContext<TModel> Create<TModel, THandler>(ServiceContext serviceContext)
        where TModel : class
        where THandler : class, IHandler<TModel>
    {
        ArgumentNullException.ThrowIfNull(serviceContext);

        return new HandlerServiceContext<TModel>(
            serviceContext,
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
    private readonly ServiceContext _serviceContext;
    private readonly Func<IServiceProvider?, IHandler<TModel>> _handlerFactory;
    private readonly Lazy<IServiceProvider?> _lazyServiceProvider;

    public HandlerServiceContext(
        ServiceContext serviceContext,
        Func<IServiceProvider?, IHandler<TModel>> handlerFactory)
    {
        _serviceContext = serviceContext;
        _handlerFactory = handlerFactory;
        _lazyServiceProvider = new Lazy<IServiceProvider?>(() => serviceContext.ServiceProviderFactory?.Invoke());
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        switch (provider: _lazyServiceProvider, context: _serviceContext)
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