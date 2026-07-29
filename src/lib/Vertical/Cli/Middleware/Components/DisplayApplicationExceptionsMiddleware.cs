using Vertical.Cli.Diagnostics;
using Vertical.Cli.Invocation;
using Vertical.Cli.IO;

namespace Vertical.Cli.Middleware.Components;

internal static class DisplayApplicationExceptionsMiddleware
{
    public static async Task InvokeAsync(InvocationContext context, Func<InvocationContext, Task> next)
    {
        try
        {
            await next(context);
        }
        catch (CommandLineException)
        {
            throw;
        }
        catch (Exception exception)
        {
            context.OutputWriter.WriteLine(exception.ToString(), DisplayElement.Important);
            context.OutputWriter.SetDisplayElement(DisplayElement.Default);
        }
    }
}