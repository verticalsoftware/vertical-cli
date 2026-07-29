using System.Runtime.Loader;
using Vertical.Cli.Invocation;

namespace Vertical.Cli.Middleware.Components;

internal static class HandleConsoleCancellationMiddleware
{
    public static async Task InvokeAsync(InvocationContext context, Func<InvocationContext, Task> next)
    {
        // SIGINT
        ConsoleCancelEventHandler sigIntHandler = (_, e) =>
        {
            e.Cancel = true;
            context.RequestCancel();
        };
        
        // SIGTERM
        Action<AssemblyLoadContext> sigTermHandler = _ => context.RequestCancel();
        
        AssemblyLoadContext.Default.Unloading += sigTermHandler;
        Console.CancelKeyPress += sigIntHandler;

        try
        {
            await next(context);
        }
        finally
        {
            Console.CancelKeyPress -= sigIntHandler;
            AssemblyLoadContext.Default.Unloading -= sigTermHandler;
        }
    }
}