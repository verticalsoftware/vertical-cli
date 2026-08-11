using Vertical.Cli.Invocation;

namespace Vertical.Cli.Middleware.Components;

internal static class HandleCommandSymbolHooksMiddleware
{
    public static async Task InvokeAsync(InvocationContext context, Func<InvocationContext, Task> next)
    {
        var (command, token) = CommandResolver.GetTarget(context.RootCommand, context.TokenList);
        var nextToken = token?.Next ?? context.TokenList.First;

        if (nextToken is null)
        {
            await next(context);
            return;
        }
        
        foreach (var symbol in command.Symbols)
        {
            if (!symbol.Aliases.Any(alias => alias.Equals(nextToken.Text)))
                continue;

            await symbol.InvokeAsync(context, command);

            if (context.Result is not null)
                return;
        }

        await next(context);
    }
}