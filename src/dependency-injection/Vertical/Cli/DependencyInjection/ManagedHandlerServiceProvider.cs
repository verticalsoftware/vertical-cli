using Vertical.Cli.Invocation;

namespace Vertical.Cli.DependencyInjection;

internal sealed class ManagedHandlerServiceProvider<TModel> : HandlerServiceProvider<TModel>
    where TModel : class
{
    private readonly IServiceProvider _serviceProvider;

    /// <inheritdoc />
    public ManagedHandlerServiceProvider(
        IServiceProvider serviceProvider,
        Func<IHandler<TModel>> handlerFactory) 
        : base(handlerFactory)
    {
        _serviceProvider = serviceProvider;
    }

    public override async ValueTask DisposeAsync()
    {
        switch (_serviceProvider)
        {
            case IAsyncDisposable asyncDisposable:
                await asyncDisposable.DisposeAsync();
                break;
            
            case IDisposable disposable:
                disposable.Dispose();
                break;
        }
    }
}