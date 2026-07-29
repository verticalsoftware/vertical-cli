using Vertical.Cli.Diagnostics;
using Vertical.Cli.Invocation;
using Vertical.Cli.Parsing;

namespace Vertical.Cli.Middleware.Components;

internal static class RouteCommandTargetMiddleware
{
    public static async Task InvokeAsync(InvocationContext context, Func<InvocationContext, Task> next)
    {
        if (!context.IsInRoutableState) 
            return;
        
        // Get the target command
        var (targetCommand, matchedToken) = CommandResolver.GetTarget(
            context.RootCommand,
            context.TokenList);

        if (!targetCommand.CanCreateCallSite)
        {
            context.AddError(new AbstractCommandError(targetCommand));
            return;
        }

        // Create a read-only token list minus command token(s)
        var tokens = matchedToken is not null
            ? context
                .TokenList
                .SkipWhile(token => !ReferenceEquals(token, matchedToken))
                .Skip(1)
            : context.TokenList;

        var tokenList = new ReadOnlyTokenList(tokens);
        var callSite = targetCommand.CreateCallSite(context, tokenList);

        context.Result = await callSite;

        await next(context);
    }
}