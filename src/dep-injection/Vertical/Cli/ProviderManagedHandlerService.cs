using Vertical.Cli.Invocation;

namespace Vertical.Cli;

internal class ProviderManagedHandlerService<TModel> : HandlerService<TModel> where TModel : class
{
    private readonly IServiceProvider _serviceProvider;

    /// <inheritdoc />
    public ProviderManagedHandlerService(
        IServiceProvider serviceProvider,
        Func<IHandler<TModel>> handlerFactory) : base(handlerFactory)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
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