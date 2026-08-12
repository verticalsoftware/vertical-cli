using Vertical.Cli.Invocation;

namespace Vertical.Cli.DependencyInjection;

internal static class ServiceProviderMiddleware
{
    public static async Task InvokeAsync(InvocationContext context, Func<InvocationContext, Task> next)
    {
        try
        {
            await next(context);
        }
        finally
        {
            var options = context
                .ApplicationOptions
                .GetOptions<DependencyInjectionOptions>();

            if (options.LazyServiceProvider.IsValueCreated)
            {
                switch (options.LazyServiceProvider.Value)
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
    }
}