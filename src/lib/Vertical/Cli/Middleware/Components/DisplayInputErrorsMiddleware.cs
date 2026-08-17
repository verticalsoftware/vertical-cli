using Vertical.Cli.Invocation;
using Vertical.Cli.IO;

namespace Vertical.Cli.Middleware.Components;

internal static class DisplayInputErrorsMiddleware
{
    public static async Task InvokeAsync(InvocationContext context, Func<InvocationContext, Task> next)
    {
        await next(context);
        
        if (context.Errors.Count == 0)
            return;
        
        var output = context.OutputWriter;
        output.SetDisplayElement(DisplayElement.Important);

        foreach (var error in context.Errors)
        {
            output.WriteLine($"{error}");
        }
        
        output.SetDisplayElement(DisplayElement.Default);
    }
}